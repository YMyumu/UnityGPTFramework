//
// Auto Generated Code By excel2json
// 1. 每个 Sheet 形成一个 Class 定义, Sheet 的名称作为 Class 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From E:\Unity\GPTFrameworkRoot\DataTables 数值表格和配置文件\Localization.xlsx.xlsx

using System.Collections.Generic;

public class Chinese_SimplifiedCfg
{
	private List<Chinese_Simplified> _cfg;
	public List<Chinese_Simplified> cfg => _cfg; // 只读属性，包含多个Chinese_Simplified对象的列表

	public Chinese_SimplifiedCfg(List<Chinese_Simplified> data)
	{
		_cfg = data;
	}
}

public class Chinese_Simplified
{
	private string _key; // 密钥
	private string _text; // 文本内容

	public string Key => _key; // 密钥
	public string Text => _text; // 文本内容

	public Chinese_Simplified(string key, string text)
	{
		_key = key;
		_text = text;
	}
}

public class EnglishCfg
{
	private List<English> _cfg;
	public List<English> cfg => _cfg; // 只读属性，包含多个English对象的列表

	public EnglishCfg(List<English> data)
	{
		_cfg = data;
	}
}

public class English
{
	private string _key; // 密钥
	private string _text; // 文本内容

	public string Key => _key; // 密钥
	public string Text => _text; // 文本内容

	public English(string key, string text)
	{
		_key = key;
		_text = text;
	}
}


// End of Auto Generated Code
