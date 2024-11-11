//
// Auto Generated Code By excel2json
// 1. 每个 Sheet 形成一个 Class 定义, Sheet 的名称作为 Class 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From E:\Unity\GPTFrameworkRoot\DataTables 数值表格和配置文件\Test_2.xlsx.xlsx

using System.Collections.Generic;

public class Sheet3Cfg
{
	private List<Sheet3> _cfg;
	public List<Sheet3> cfg => _cfg; // 只读属性，包含多个Sheet3对象的列表

	public Sheet3Cfg(List<Sheet3> data)
	{
		_cfg = data;
	}
}

public class Sheet3
{
	private int _id; // 编号
	private string _name; // 名称
	private string _assetname; // 资源编号
	private int _hp; // 血
	private int _attack; // 攻击
	private int _defence; // 防御
	private string _datetest; // 测试日期
	private List<string> _testjsonarray; // 测试单元格内的Json数组
	private Dictionary<string, string> _testjsonobject; // 测试单元格内的Json对象

	public int ID => _id; // 编号
	public string Name => _name; // 名称
	public string AssetName => _assetname; // 资源编号
	public int HP => _hp; // 血
	public int Attack => _attack; // 攻击
	public int Defence => _defence; // 防御
	public string DateTest => _datetest; // 测试日期
	public List<string> TestJsonArray => _testjsonarray; // 测试单元格内的Json数组
	public Dictionary<string, string> TestJsonObject => _testjsonobject; // 测试单元格内的Json对象

	public Sheet3(int id, string name, string assetname, int hp, int attack, int defence, string datetest, List<string> testjsonarray, Dictionary<string, string> testjsonobject)
	{
		_id = id;
		_name = name;
		_assetname = assetname;
		_hp = hp;
		_attack = attack;
		_defence = defence;
		_datetest = datetest;
		_testjsonarray = testjsonarray;
		_testjsonobject = testjsonobject;
	}
}

public class Sheet2Cfg
{
	private List<Sheet2> _cfg;
	public List<Sheet2> cfg => _cfg; // 只读属性，包含多个Sheet2对象的列表

	public Sheet2Cfg(List<Sheet2> data)
	{
		_cfg = data;
	}
}

public class Sheet2
{
	private int _id; // 编号
	private string _name; // 名称
	private string _assetname; // 资源编号
	private int _hp; // 血
	private int _attack; // 攻击
	private int _defence; // 防御
	private string _datetest; // 测试日期

	public int ID => _id; // 编号
	public string Name => _name; // 名称
	public string AssetName => _assetname; // 资源编号
	public int HP => _hp; // 血
	public int Attack => _attack; // 攻击
	public int Defence => _defence; // 防御
	public string DateTest => _datetest; // 测试日期

	public Sheet2(int id, string name, string assetname, int hp, int attack, int defence, string datetest)
	{
		_id = id;
		_name = name;
		_assetname = assetname;
		_hp = hp;
		_attack = attack;
		_defence = defence;
		_datetest = datetest;
	}
}


// End of Auto Generated Code
