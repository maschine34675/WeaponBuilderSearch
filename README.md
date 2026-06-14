\# Weapon Builder Search



Adds a live search field to the attachment dropdown in the \*\*Weapon Builder\*\* (Edit Build) and \*\*Weapon Modding\*\* screens.



When a slot has many compatible parts — especially with weapon/content mods — you can filter the list instantly instead of scrolling through dozens of entries.



\## Features



\- Real-time filtering as you type

\- Matches \*\*short name\*\* and \*\*full item name\*\* (case-insensitive)

\- Search bar appears below the attachment list

\- Only shows when a slot has enough options (configurable minimum)

\- Compatible with \*\*\[SPTScrollableAttachments](https://github.com/peinwastaken/SPTScrollableAttachments)\*\* (search stays visible inside the scroll viewport)



\## Configuration



`BepInEx/config/com.maschine.weaponbuildersearch.cfg`



| Setting | Default | Description |

|---------|---------|-------------|

| `Enabled` | `true` | Enable or disable the mod |

| `MinItemsForSearch` | `6` | Minimum number of attachments before the search field appears |



\## Installation



1\. Install \[BepInEx for SPT](https://hub.sp-tarkov.com/files/file/2-bepinex/)

2\. Drop `maschine-WeaponBuilderSearch.dll` into `BepInEx/plugins/`

3\. Start the game



\*\*Client-side only\*\* — no server install required.



\## Requirements



\- SPT with BepInEx

