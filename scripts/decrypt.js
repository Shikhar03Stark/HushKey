const { subtle } = globalThis.crypto;
import { argv } from "node:process";

function getKeysFromUrl(url) {
    const urlObj = new URL(url);
    const fragment = urlObj.hash.substring(1); // Remove the leading '#'
    const [nonce, key, tag] = fragment.split('.');
    if (!nonce || !key || !tag) {
        throw new Error(`Invalid URL format ${fragment}. Expected format: #nonce:key:tag`);
    }
    const keys = {
        nonce: Uint8Array.from(atob(nonce), c => c.charCodeAt(0)),
        key: Uint8Array.from(atob(key), c => c.charCodeAt(0)),
        tag: Uint8Array.from(atob(tag), c => c.charCodeAt(0))
    };
    console.log(`BitLength of nonce: ${keys.nonce.length*8}, key: ${keys.key.length*8}, tag: ${keys.tag.length*8}`);
    return keys;
}

function bytesToBase64(bytes) {
    return btoa(String.fromCharCode(...bytes));
}

function base64ToBytes(base64) {
    return Uint8Array.from(atob(base64), c => c.charCodeAt(0));
}

async function extractSecret(url) {
    console.log("Extracting secret key from URL:", url);
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Failed to fetch URL: ${response.statusText}`);
        }
        const {nonce, key, tag} = getKeysFromUrl(url);
        const data = await response.json();
    
        // append tag to secret of base64 string
        const encryptedSecret = base64ToBytes(data.encryptedSecret);
        const fullCiphertext = new Uint8Array(encryptedSecret.length + tag.length);
        fullCiphertext.set(encryptedSecret);
        fullCiphertext.set(tag, encryptedSecret.length);
        console.log("Full ciphertext length:", fullCiphertext.length);

        if (!encryptedSecret) {
            throw new Error("No encrypted secret found in the response.");
        }
    
        console.log("Decrypting secret...");
        const decrypted = await subtle.decrypt(
            {
                name: "AES-GCM",
                iv: nonce,
                tagLength: tag.length * 8 // Convert bytes to bits
            },
            await subtle.importKey("raw", key, "AES-GCM", false, ["decrypt"]),
            fullCiphertext
        );
    
        console.log("Decryption successful.");
        return new TextDecoder().decode(decrypted);
        
    } catch (error) {
        throw new Error(error);
    }
}

async function main() {
    process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
    if (argv.length < 3) {
        console.error("Usage: node decrypt.js <url>");
        process.exit(1);
    }

    const url = argv[2];
    try {
        const secret = await extractSecret(url);
        console.log("Decrypted secret:", secret);
    } catch (error) {
        console.error("Error:", error);
        throw new Error(`Failed to extract secret: ${error.message}`);
    }
}

main().catch(error => {
    console.error("Unexpected error:", error);
    process.exit(1);
});