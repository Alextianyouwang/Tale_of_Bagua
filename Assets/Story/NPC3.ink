EXTERNAL GeneralEvent (name,onlyAllowOnceGlobally)
=== 0Interaction ===
Yo glad you find me on the second level!
But I can't talk to you right now...
-> DONE

=== 1Interaction ===
Not bad finding me on the second level. I am NPC3,
My existance is souly for erasing the memory of NPC1.
->choice
=== choice === 
What say, do you want me to erase it?
    * [Yes Please Do So!]
        ..................
        ((((@_@))))
        _+_+_++_+_+_+++
        Done!
        NPC1 Memory erased! {GeneralEvent("EraseMemory",true)}
        ->DONE
    * [Nah, its not nice]
        Ay, nothing will happen.
        ->DONE
    * [I don't believe you your random ass dude]
        I give you another chance to say it properly.
        ->choice
        -> DONE
=== 2Interaction ===
As long as I got the permission from someone to erase another NPC's memory, I will be more powerful than before EHHHEAHAHAHAHAHAHHHA!!!!!!!
SYSTEM MESSAGE : You have experienced all possible interactions with this NPC.
-> DONE
=== Fallback ===
I have nothing to say at this moment. Please Check your script.
-> END