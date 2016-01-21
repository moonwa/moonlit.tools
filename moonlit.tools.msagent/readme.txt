添加 Microsoft Agent Control
如果要在网页上使用 msagent, 请使用
<object style="visibility:hidden" id="MSAgent" 
classid="CLSID:D45FD31B-5C6E-11D1-9EC1-00C04FD7081F"></object> 
<script language="JavaScript">
//Coded by Windy_sk <windy_sk@126.com> 20040214

var Agent = null;
var AgentID = "Merlin";
var AgentACS = "merlin.acs";
MSAgent.Connected = true;
MSAgent.Characters.Load(AgentID,AgentACS);
Agent = MSAgent.Characters.Character(AgentID);
Agent.Show();
</script>

自动安装
方法一. <object style="visibility:hidden" id="MSAgent" classid="CLSID:D45FD31B-5C6E-11D1-9EC1-00C04FD7081F" CodeBase="http://activex.microsoft.com/activex/controls/agent2/MSagent.exe#VERSION=2,0,0,0"></object>
方法二. <script language="javascript">
		//Coded by Windy_sk <windy_sk@126.com> 20040214

		function Agent_load_error(){
			alert("To make the MSAgent available, /nplease install Microsoft Agent core components first !");
			window.open("http://activex.microsoft.com/activex/controls/agent2/MSagent.exe");
			return;
		}
		</script>
		<object style="visibility:hidden" id="MSAgent" classid="CLSID:D45FD31B-5C6E-11D1-9EC1-00C04FD7081F" onerror="Agent_load_error()"></object>
