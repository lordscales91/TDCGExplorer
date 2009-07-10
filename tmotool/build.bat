csc /out:build\TMOMove.exe @tmotool.rsp Move.cs TMOMove.cs
csc /out:build\TMORotY.exe @tmotool.rsp RotY.cs TMORotY.cs
csc /out:build\TMOZoom.exe @tmotool.rsp Zoom.cs TMOZoom.cs
csc /out:build\TMOBoin.exe @tmotool.rsp TMOTool\ITMOCommand.cs TMOTool\command\Boin.cs TMOBoin.cs
csc /out:build\TMODom.exe @tmotool.rsp TMOTool\ITMOCommand.cs TMOTool\command\Dom.cs TMODom.cs
csc /out:build\TMOPedo.exe @tmotool.rsp TMOTool\ITMOCommand.cs TMOTool\command\Pedo.cs TMOPedo.cs
csc /out:build\TMONodeCopy.exe @tmotool.rsp TMONodeCopy.cs
csc /out:build\TMONodeDump.exe @tmotool.rsp TMONodeDump.cs
csc /out:build\TMOPose.exe @tmotool.rsp TMOPose.cs
csc /out:build\TMOAppend.exe @tmotool.rsp TMOAppend.cs
csc /out:build\TMOMotionCopy.exe @tmotool.rsp TMOMotionCopy.cs
csc /out:build\TMOAnim.exe @tmotool.rsp TMOAnim.cs
csc /out:build\TMOTool.exe @tmotool.rsp /r:CSScriptLibrary.dll TMOTool\Program.cs
