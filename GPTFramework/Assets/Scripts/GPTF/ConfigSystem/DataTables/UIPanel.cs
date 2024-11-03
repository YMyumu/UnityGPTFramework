//
// Auto Generated Code By excel2json
// 1. 每个 Sheet 形成一个 Class 定义, Sheet 的名称作为 Class 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From E:\Unity\GPTFrameworkRoot\DataTables 数值表格和配置文件\UIPanel.xlsx.xlsx

using System.Collections.Generic;

public class UIPanelsCfg
{
	private List<UIPanels> _cfg;
	public List<UIPanels> cfg => _cfg; // 只读属性，包含多个UIPanels对象的列表

	public UIPanelsCfg(List<UIPanels> data)
	{
		_cfg = data;
	}
}

public class UIPanels
{
	private string _uiname; // UI面板名称
	private string _path; // UI预制件路径
	private string _uilayer; // 所属层级
	private bool _isclosesamelayer; // 是否关闭同层其他界面
	private bool _isfullpanel; // 是否是全屏界面
	private bool _ismodernpanel; // 是否是模态窗口
	private bool _isclickmodernneedclose; // 点击模态窗口是否可以关闭界面

	public string UIName => _uiname; // UI面板名称
	public string Path => _path; // UI预制件路径
	public string UILayer => _uilayer; // 所属层级
	public bool IsCloseSameLayer => _isclosesamelayer; // 是否关闭同层其他界面
	public bool IsFullPanel => _isfullpanel; // 是否是全屏界面
	public bool IsModernPanel => _ismodernpanel; // 是否是模态窗口
	public bool IsClickModernNeedClose => _isclickmodernneedclose; // 点击模态窗口是否可以关闭界面

	public UIPanels(string uiname, string path, string uilayer, bool isclosesamelayer, bool isfullpanel, bool ismodernpanel, bool isclickmodernneedclose)
	{
		_uiname = uiname;
		_path = path;
		_uilayer = uilayer;
		_isclosesamelayer = isclosesamelayer;
		_isfullpanel = isfullpanel;
		_ismodernpanel = ismodernpanel;
		_isclickmodernneedclose = isclickmodernneedclose;
	}
}


// End of Auto Generated Code
