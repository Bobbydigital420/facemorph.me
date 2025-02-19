module Config

let videoDim = 1024
let imgDim = 1024
let imgSizesSet = [ 300; 512; 1024 ]
let linkpreviewWidth = 1200
let linkpreviewHeight = 628
let ogImgDim = 1024
let ogVideoDim = 1024
let maxSupportedImgDim = 1024
let thumbnailDim = 128

let siteName = "facemorph.me"
let canonicalBaseUrl = "https://facemorph.me"
let oEmbedApiEndpoint = canonicalBaseUrl + "/oembed.json"
let contactEmail = "checkfaceml@gmail.com"
let githubRepo = "check-face/facemorph.me"

let defaultTextValue = "hello"
let defaultNumericSeed = 389u // 389 is one of the seeds featured in the stylegan2 paper

let apiAddr = "https://api.facemorph.me"
let encodeApiAddr = apiAddr + "/api/encodeimage/"

let browseFacesEmbedSrc = "https://names.facemorph.me"
