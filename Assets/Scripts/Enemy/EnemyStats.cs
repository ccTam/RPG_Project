using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Unit/Enemy")]
public class Enemy : ScriptableObject
{
	[SerializeField]
	private float HP, MP, SP;
	[SerializeField]
	private float cHP, cMP, cSP;
	[SerializeField]
	private float HPR, MPR, SPR;
	[SerializeField]
	private float HPR_Multiplier, MPR_Multiplier, SPR_Multiplier;
	[SerializeField]
	private float Str, Dex, Int, Will, Luck;
	private float PAmin, PAmax, MA, CritRate, CritDamage, PDef, PPro, MDef, MPro, AP;

	[SerializeField]
	private bool canControl, isAlive, isDeadly, isImmune, isStatic;
}
