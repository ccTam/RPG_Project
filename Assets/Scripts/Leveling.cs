using UnityEngine;

public class Leveling : MonoBehaviour
{
	[SerializeField]
	private int[] requiredExp;
	private const int MAXLEVEL = 200;

	public int[] RequiredExp { get { return requiredExp; } }

	void Start()
	{
		requiredExp = new int[MAXLEVEL + 1];
		SetupExpChart();
	}

	void SetupExpChart()
	{
		requiredExp[0] = 0;
		requiredExp[1] = 400;
		for (int i = 2; i < requiredExp.Length; i++)
			requiredExp[i] = requiredExp[i - 1] + (int)(200 * Mathf.Pow(i, 1.35f));
	}

	public int ExpToLevel(int curExp, int i)
	{
		if (i == 0)
			i = 1;
		for (; i < requiredExp.Length; i++)
		{
			if (curExp < requiredExp[i] && curExp >= requiredExp[i - 1])
			{
				return i - 1;
			}
		}
		return MAXLEVEL;
	}

	public int GetRemainExp(int curExp, int i)
	{
		if (i != requiredExp.Length - 1)
			return requiredExp[i + 1] - curExp;
		else
			return 0;
	}

	public int LvRequiredExp(int lv)
	{
		if (lv > requiredExp.Length - 1 || lv < 0)
			return 0;
		return requiredExp[lv];
	}
}
