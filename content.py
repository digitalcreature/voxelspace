import os
contentRoot = "./Content/"
contentConfig = "./Content/Content.mgcb"

header = """
#----------------------------- Global Properties ----------------------------#

/outputDir:bin/$(Platform)
/intermediateDir:obj/$(Platform)
/platform:DesktopGL
/config:
/profile:Reach
/compress:False

#-------------------------------- References --------------------------------#


#---------------------------------- Content ---------------------------------#
"""

templates = {
    ".png": """
#begin {0}
/importer:TextureImporter
/processor:TextureProcessor
/processorParam:ColorKeyColor=255,0,255,255
/processorParam:ColorKeyEnabled=True
/processorParam:GenerateMipmaps=False
/processorParam:PremultiplyAlpha=True
/processorParam:ResizeToPowerOfTwo=False
/processorParam:MakeSquare=False
/processorParam:TextureFormat=Color
/build:{0}
    """,
    ".fx": """
#begin {0}
/importer:EffectImporter
/processor:EffectProcessor
/processorParam:DebugMode=Auto
/build:{0}
"""
}

with open(contentConfig, "w") as output:
    output.write(header)
    for file in (os.path.join(dp, f) for dp, dn, filenames in os.walk(contentRoot) for f in filenames):
        file = file.replace(contentRoot, "")
        _, ext = os.path.splitext(file)
        if ext in templates:
            output.write(templates[ext].format(file))

    
