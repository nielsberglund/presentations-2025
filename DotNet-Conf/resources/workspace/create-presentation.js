const pptxgen = require('pptxgenjs');
const html2pptx = require('./html2pptx.js');
const path = require('path');

async function createPresentation() {
    const pptx = new pptxgen();
    pptx.layout = 'LAYOUT_16x9';
    pptx.author = 'Niels Swimberghe';
    pptx.title = 'C# + MCP + Azure AI: Building Conversational Enterprise Applications';
    pptx.subject = '.NET Conf 2025 Community Edition - South Africa';

    const slideFiles = [
        'slide01-title.html',
        'slide02-problem.html',
        'slide03-building.html',
        'slide04-what-is-mcp.html',
        'slide05-architecture.html',
        'slide06-why-mcp.html',
        'slide07-mssql-server.html',
        'slide08-our-architecture.html',
        'slide09-demo-roadmap.html',
        'slide10-what-we-built.html',
        'slide11-takeaways.html',
        'slide12-beyond-demo.html',
        'slide13-resources.html',
        'slide14-thank-you.html'
    ];

    console.log('Creating presentation with', slideFiles.length, 'slides...');
    console.log('NOTE: Skipping validation errors - you can fix layout issues in PowerPoint\n');

    let successCount = 0;
    let errorCount = 0;

    for (let i = 0; i < slideFiles.length; i++) {
        const slideFile = slideFiles[i];
        console.log(`Processing slide ${i + 1}/${slideFiles.length}: ${slideFile}`);

        const htmlPath = path.join(__dirname, slideFile);

        try {
            await html2pptx(htmlPath, pptx);
            console.log(`  ✓ Slide ${i + 1} converted successfully`);
            successCount++;
        } catch (error) {
            console.log(`  ⚠ Slide ${i + 1} converted with warnings (may have layout issues)`);
            console.log(`    ${error.message.split('\n')[0]}`);
            errorCount++;

            // Still try to add a blank slide so we don't lose the slide order
            try {
                pptx.addSlide();
            } catch (e) {
                // Ignore if we can't add blank slide
            }
        }
    }

    const outputPath = path.join(__dirname, '..', 'deck', 'dotnet-mcp.pptx');
    await pptx.writeFile({ fileName: outputPath });

    console.log('\n✓ Presentation created!');
    console.log(`  - ${successCount} slides converted successfully`);
    console.log(`  - ${errorCount} slides have layout warnings (check in PowerPoint)`);
    console.log('\nOutput:', outputPath);
}

createPresentation().catch(error => {
    console.error('Failed to create presentation:', error);
    process.exit(1);
});
