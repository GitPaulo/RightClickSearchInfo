using Dalamud.Game.ClientState.Objects.SubKinds;

namespace RightClickSearchInfo.Services;

public class SearchInfoCommandService(Plugin plugin)
{
   public string CreateCommandString(PlayerCharacter? target)
   { 
       var targetFullName = target!.Name.ToString(); 
       var targetNameSplit = targetFullName.Split(' '); 
       var searchCommand = $"/search forename \"{targetNameSplit[0]}\" surname \"{targetNameSplit[1]}\"";
       return searchCommand;
   }
   
   public string CreateCommandString(string targetFullName)
   {
       var targetNameSplit = targetFullName.Split(' ');
       var searchCommand = $"/search forename \"{targetNameSplit[0]}\" surname \"{targetNameSplit[1]}\"";
       return searchCommand;
   }
}
