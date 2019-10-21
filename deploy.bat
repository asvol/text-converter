SET BASH_PATH=%SYSTEMDRIVE%\Users\%USERNAME%\.babun\cygwin\bin\
SET PATH=%PATH%;%BASH_PATH%

rsync -r -v ./bin/publish/* root@asv.me:/var/www/text-converter/
