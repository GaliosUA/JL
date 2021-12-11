# JL
JL is a program for looking up Japanese words and expressions. It can also be used for word/sentence mining. Inspired by [Nazeka](https://github.com/wareya/nazeka) and [Chiitrans Lite](https://github.com/alexbft/chiitrans).

Download from the [releases page](https://github.com/rampaa/JL/releases). Prefer the x86 version for 50-80% less memory usage.

## System requirements
* Windows 7 or later
* .NET Desktop Runtime 6.0 or later

## Features
* Highly customizable
* Anki mining
* Pass-through mode 

## Supported dictionaries

### EDICT

* JMdict - Full support
* JMnedict - Full support
* KANJIDIC - Full support (w/ composition data)

### EPWING ([yomichan-import](https://github.com/FooSoft/yomichan-import/) format)

* Daijirin - Basic+ support
* Daijisen - Basic support
* Koujien - Basic support
* Meikyou - Basic support
* Kenkyuusha - Basic support

## Resources used
* [Nazeka](https://github.com/wareya/nazeka): Deconjugation rules, deconjugator, frequency lists
* [JMdict](https://www.edrdg.org/wiki/index.php/JMdict-EDICT_Dictionary_Project): JMdict_e.gz
* [JMnedict](https://www.edrdg.org/enamdict/enamdict_doc.html): JMnedict.xml.gz
* [KANJIDIC](https://www.edrdg.org/wiki/index.php/KANJIDIC_Project): kanjidic2.xml.gz
* [cjkvi-ids](https://github.com/cjkvi/cjkvi-ids): ids.txt

## Screenshots (TODO)


## FAQ
### Why can't I look anything up?
Make sure you're not in pass-through mode, kanji mode, have lookup-on-select-only enabled, or have disabled lookups (default: Q key).
### How do I disable pass-through mode?
Press the opacity slider button located top-left of the main window.
### How do I add EPWING dictionaries?
Select the folder containing the **unzipped** (so you should have a folder of files named like term_bank_1.json, term_bank_2.json...)  contents of a dictionary converted with [yomichan-import](https://github.com/FooSoft/yomichan-import/), on the Manage Dictionaries window.
### Where are my settings stored?
* Anki settings: Config/AnkiConfig.json
* Dictionary settings: Config/dicts.json
* Everything else: JL.dll.config
### Will you add machine translation capabilities?
No.

## License
TBD
