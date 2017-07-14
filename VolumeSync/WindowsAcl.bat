@echo off
netsh http add urlacl url=http://+:57564/ user=Everyone
netsh advfirewall firewall add rule name="VolumeSync" dir=in action=allow protocol=TCP localport=57564
