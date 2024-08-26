// wwwroot/site.js
function downloadPdf(base64Data, fileName) {
    const link = document.createElement('a');
    link.href = `data:application/pdf;base64,${base64Data}`;
    link.download = fileName;
    link.click();
}
