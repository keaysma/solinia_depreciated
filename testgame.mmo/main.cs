//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Load up common script base
loadDir("common");

//-----------------------------------------------------------------------------
// Load up defaults console values.

// Defaults console values
exec("./client/defaults.cs");
exec("./server/defaults.cs");

// Preferences (overide defaults)
exec("./client/prefs.cs");
exec("./server/prefs.cs");
exec("./datablocks/init.cs");


//-----------------------------------------------------------------------------
// Package overrides to initialize the mod.
package FpsStarterKit {

function displayHelp() {
   Parent::displayHelp();
   error(
      "FPS Mod options:\n"@
      "  -dedicated             Start as dedicated server\n"@
      "  -connect <address>     For non-dedicated: Connect to a game at <address>\n" @
      "  -mission <filename>    For dedicated: Load the mission\n"
   );
}

function parseArgs()
{
   Parent::parseArgs();

   // Arguments, which override everything else.
   for (%i = 1; %i < $Game::argc ; %i++)
   {
      %arg = $Game::argv[%i];
      %nextArg = $Game::argv[%i+1];
      %hasNextArg = $Game::argc - %i > 1;
   
      switch$ (%arg)
      {
         //--------------------
         case "-dedicated":
            $Server::Dedicated = true;
            enableWinConsole(true);
            $argUsed[%i]++;

         //--------------------
         case "-mission":
            $argUsed[%i]++;
            if (%hasNextArg) {
               $missionArg = %nextArg;
               $argUsed[%i+1]++;
               %i++;
            }
            else
               error("Error: Missing Command Line argument. Usage: -mission <filename>");

         case "-zone":
            $argUsed[%i]++;
            if (%hasNextArg) {
               $zoneArg = %nextArg;
               $argUsed[%i+1]++;
               %i++;
            }
            else
               error("Error: Missing Command Line argument. Usage: -mission <filename>");


         case "-world":
            $argUsed[%i]++;
            if (%hasNextArg) {
               $Py::DedicatedWorld = %nextArg;
               $argUsed[%i+1]++;
               %i++;
            }
            else
               error("Error: Missing Command Line argument. Usage: -mission <filename>");



         case "-serverport":
            $argUsed[%i]++;
            if (%hasNextArg) {
               $Pref::Server::Port = %nextArg;
               $argUsed[%i+1]++;
               %i++;
            }
            else
               error("Error: Missing Command Line argument. Usage: -serverport port#");

         case "-worldport":
            $argUsed[%i]++;
            if (%hasNextArg) {
               $Py::WorldPort = %nextArg;
               $argUsed[%i+1]++;
               %i++;
            }
            else
               error("Error: Missing Command Line argument. Usage: -serverport port#");

         //--------------------
         case "-connect":
            $argUsed[%i]++;
            if (%hasNextArg) {
               $JoinGameAddress = %nextArg;
               $argUsed[%i+1]++;
               %i++;
            }
            else
               error("Error: Missing Command Line argument. Usage: -connect <ip_address>");
      }
   }
}

function onStart()
{
   Parent::onStart();
   echo("\n--------- Initializing MOD: FPS Starter Kit ---------");

   // Load the scripts that start it all...
   exec("./client/init.cs");
   exec("./server/init.cs");
   exec("./data/init.cs");

   // Server gets loaded for all sessions, since clients
   // can host in-game servers.
   initServer();

   // Start up in either client, or dedicated server mode
   if ($Server::Dedicated)
      initDedicated();
   else
      initClient();
   
   if( isTMKenabled() )
   {
   //------------------------------------------------------------------
   // TGE Modernization Kit
   //------------------------------------------------------------------
   
   //exec("./data/init.cs");
   DRL::setBiasLimits(-0.05, 0);
   DRL::setGoalIntensity(0.35);
   DRL::setBloomColorOffset(0.15);
   
   //------------------------------------------------------------------
   // TGE Modernization Kit
   //------------------------------------------------------------------
   }
}

function onExit()
{
   if (!$Server::Dedicated)
   {
	   echo("Exporting client prefs");
	   export("$pref::*", "./client/prefs.cs", False);

	   echo("Exporting client config");
	   if (isObject(moveMap))
		  moveMap.save("./client/config.cs", false);

	   echo("Exporting server prefs");
	   export("$Pref::Server::*", "./server/prefs.cs", False);
	   BanList::Export("./server/banlist.cs");
   }

   Parent::onExit();
   
}

}; // Client package
activatePackage(FpsStarterKit);
