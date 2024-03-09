async function copyToClipboard(img) {
    await navigator.clipboard.write([
        new ClipboardItem({
             "text/plain": new Blob([img.src], { type: "text/plain" }),
             "image/png": getPNGBlob(img),
        })
    ]);
}

const getPNGBlob = async img => {
    const size = 128;
    
    const canvas = new OffscreenCanvas(size, size);
    const ctx = canvas.getContext("2d");
    
    ctx.drawImage(img, 0, 0, size, size);

    return await canvas.convertToBlob();
};

window.addEventListener("DOMContentLoaded", () => {
    document
        .querySelectorAll("div.icon img")
        .forEach(icon => icon.addEventListener("click", event => copyToClipboard(event.target)));
});