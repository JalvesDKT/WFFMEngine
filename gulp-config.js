module.exports = function () {
    var instanceRoot = "C:\DigitalKT\local.wffmengine.dev";
    var config = {
        websiteRoot: instanceRoot + "\\Website",
        sitecoreLibraries: instanceRoot + "\\Website\\bin",
        licensePath: instanceRoot + "\\Data\\license.xml",
        solutionName: "WFFMEngine",
        buildConfiguration: "Debug",
        runCleanBuilds: false
    };
    return config;
}