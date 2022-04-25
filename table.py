import json
from bs4 import BeautifulSoup as bs

class Collection:
    def __init__(self, name, lowestRarity, skins, link):
        self.name = name
        self.lowestRarity = lowestRarity
        self.skins = skins
        self.link = link
    def __repr__(self):
        return f"{self.name}"

class Skin:
    def __init__(self, name, rarity, minWear, maxWear):
        self.name = name
        self.rarity = rarity
        self.minWear = minWear
        self.maxWear = maxWear
    def __repr__(self):
        return f"{self.name} ({self.minWear}-{self.maxWear})"

def splitByRange(skins):
    ranges = {}
    for skin in skins:
        currentRange = (skin.minWear, skin.maxWear)
        if currentRange not in ranges:
            ranges[currentRange] = []
        ranges[currentRange].append(skin)
    return ranges

colorCodes = {
    "Covert": "danger",
    "Classified": "success",
    "Restricted": "primary",
    "Mil-Spec": "info",
    "Industrial": "secondary"
}

def getRarity(rarity, collection):
    if rarity not in collection:
        return ""

    bars = []
    splitted = splitByRange(collection[rarity])
    totalPercent = 0
    index = 0
    for range in splitted:
        percent = round(len(splitted[range]) / len(collection[rarity]) * 100)
        
        widthPercent = percent
        if len(splitted) == 3 and widthPercent == 33 and index == 1:
            widthPercent += 1
        index += 1
        totalPercent += widthPercent

        skinList = []
        for skin in splitted[range]:
            skinList.append(f"<li>{skin.name}</li>")

        bars.append(f"""
        <div class="progress-bar progress-bar-striped bg-{colorCodes[rarity]}" 
            data-toggle="tooltip" data-html="true" 
            title="<em>{range[0]} - {range[1]}</em><br><ul class='list-unstyled'>{"".join(skinList)}</ul>" 
            role="progressbar" style="width: {widthPercent}%" aria-valuenow="{widthPercent}" aria-valuemin="0" aria-valuemax="100">{percent}%</div>
        """)
    
    return f"""
    <span>{rarity}:</span>
    <div class="progress">
        {''.join(bars)}
    </div>
    """

if __name__ == "__main__":
    collections = []
    template = ""
    with open("SkinList.json", encoding="utf-8") as f:
        all = json.load(f)
        for i in all:
            skins = []
            for skin in i["Skins"]:
                if skin["Rarity"] != i["LowestRarity"]:
                    skins.append(Skin(skin["Name"], skin["Rarity"], skin["MinWear"], skin["MaxWear"]))
            collections.append(Collection(i["Name"], i["LowestRarity"], skins, i["Link"]))

    rowCounter = 0
    root = ""
    with open("output.html", "w+", encoding="utf-8") as h:
        for collection in collections:
            skinsByRarity = {}
            for skin in collection.skins:
                if skin.rarity not in skinsByRarity:
                    skinsByRarity[skin.rarity] = []
                skinsByRarity[skin.rarity].append(skin)


            if rowCounter % 3 == 0:
                root += '<div class="row">\n'

            root += f"""
            <div class="col-md-4">
                <div class="text-center">
                    <img src="Cases/{collection.name.replace(" ", "").replace(":","").replace(".","")}.webp" class="img">
                    <h4>{collection.name}</h4>
                </div>
                <div>
                    {getRarity("Covert", skinsByRarity)}
                    {getRarity("Classified", skinsByRarity)}
                    {getRarity("Restricted", skinsByRarity)}
                    {getRarity("MilSpec", skinsByRarity)}
                    {getRarity("Industrial", skinsByRarity)}
                </div>
                <hr>
                <p>
                    <a class="btn btn-secondary" href="{collection.link}" role="button">Details &raquo;</a>
                </p>
            </div>
            """

            rowCounter += 1

            if rowCounter % 3 == 0:
                root += '<hr>\n</div>\n<hr>\n'

        soup = bs(root, features="html.parser")
        h.write(soup.prettify())