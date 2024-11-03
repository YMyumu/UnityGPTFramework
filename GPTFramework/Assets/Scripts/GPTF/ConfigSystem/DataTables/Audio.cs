//
// Auto Generated Code By excel2json
// 1. 每个 Sheet 形成一个 Class 定义, Sheet 的名称作为 Class 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From E:\Unity\GPTFrameworkRoot\DataTables 数值表格和配置文件\Audio.xlsx.xlsx

using System.Collections.Generic;

public class MixerGroupDataCfg
{
	private List<MixerGroupData> _cfg;
	public List<MixerGroupData> cfg => _cfg; // 只读属性，包含多个MixerGroupData对象的列表

	public MixerGroupDataCfg(List<MixerGroupData> data)
	{
		_cfg = data;
	}
}

public class MixerGroupData
{
	private string _groupname; // 组名
	private float _initialvolume; // 初始音量
	private string _parentgroup; // 父组

	public string GroupName => _groupname; // 组名
	public float InitialVolume => _initialvolume; // 初始音量
	public string ParentGroup => _parentgroup; // 父组

	public MixerGroupData(string groupname, float initialvolume, string parentgroup)
	{
		_groupname = groupname;
		_initialvolume = initialvolume;
		_parentgroup = parentgroup;
	}
}

public class AudiosCfg
{
	private List<Audios> _cfg;
	public List<Audios> cfg => _cfg; // 只读属性，包含多个Audios对象的列表

	public AudiosCfg(List<Audios> data)
	{
		_cfg = data;
	}
}

public class Audios
{
	private string _name; // 音频名字
	private string _path; // 音频路径
	private bool _isloop; // 音频是否循环
	private string _mixername; // 音频组

	public string Name => _name; // 音频名字
	public string Path => _path; // 音频路径
	public bool IsLoop => _isloop; // 音频是否循环
	public string MixerName => _mixername; // 音频组

	public Audios(string name, string path, bool isloop, string mixername)
	{
		_name = name;
		_path = path;
		_isloop = isloop;
		_mixername = mixername;
	}
}


// End of Auto Generated Code
