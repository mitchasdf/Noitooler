The executable is located in /Noitooler/bin/release/ -> "Noitooler.exe", you can just download that single file and run that. It should trigger security programs just like Cheat Engine does, just be warned.

If noita gets updated, this repo might certainly break. In which case, the memory offset for the seed will need changing. To find the new seed location, use a memory editor like Cheat Engine to look for the seed as a 4 byte integer. There should be a read-only address at "Noita.exe+EF2814" (the hexadecimal number here is just an example), that hex number is where the seed will be stored.