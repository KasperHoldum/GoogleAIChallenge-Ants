CD\
CD C:\Users\Mira\Documents\Visual Studio 2010\Projects\Qua.AntsBot\Qua.Ants.TestBot\
ECHO Y | DEL MyBot.Zip
"c:\program files\winrar\winrar" a -afzip MyBot.zip "*.cs" 

CD C:\Users\Mira\Documents\Visual Studio 2010\Projects\Qua.AntsBot\Qua.Ants\
"c:\program files\winrar\winrar" a -afzip "C:\Users\Mira\Documents\Visual Studio 2010\Projects\Qua.AntsBot\Qua.Ants.TestBot\MyBot.zip" "*.cs"

CD C:\Users\Mira\Documents\Visual Studio 2010\Projects\Qua.AntsBot\Qua.Ants\DataStructures
"c:\program files\winrar\winrar" a -afzip "C:\Users\Mira\Documents\Visual Studio 2010\Projects\Qua.AntsBot\Qua.Ants.TestBot\MyBot.zip" "*.cs"

CD C:\Users\Mira\Documents\Visual Studio 2010\Projects\Qua.AntsBot\Qua.Ants.TestBot\