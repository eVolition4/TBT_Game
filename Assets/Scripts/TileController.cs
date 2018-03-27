﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public bool isOccupied = false;
    //public int xPos, yPos;
    public IntVector2 curCoords;
    public Material startingMat, hoverMat, movementMat, attackMat;
    public GameObject unitOnTile;

    GameController gameController;

    void Awake()
    {
        gameController = GameController.instance;
    }

    void Start()
    {
        startingMat = gameObject.GetComponent<MeshRenderer>().material;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Debug.Log("Pointer Entering tile!");
        HoverTileEnter();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        HoverTileExit();
    }
 
    void HoverTileEnter()
    {
        //ChangeTileMaterial(hoverMat);
        gameController.curHoveredTile = this;
        gameController.cursor.transform.position = gameObject.transform.position + new Vector3 (0f, 0.00001f, 0f);
    }

    void HoverTileExit()
    {
        //ChangeTileMaterial(startingMat);
        //gameController.curHoveredTile = null;
    }

    public void ChangeTileMaterial(Material newMat)
    {
        gameObject.GetComponent<MeshRenderer>().material = newMat;
    }

    public void OnTileSelect()
    {
        gameController.curSelectedTile = this;

        if (gameController.curUnit != null)
        {
            //Debug.Log("Deselecting GO with ID: " + gameController.curUnit.GetInstanceID());

            gameController.curUnit.OnUnitDeselect();
            gameController.curUnit = null;
        }

        if(unitOnTile != null)
        {
            gameController.curUnit = unitOnTile.GetComponent<UnitController>();
            if(gameController.curUnit != null)
            {
                //Unit on tile has a UnitController script
                gameController.curUnit.OnUnitSelect();
            }

            else
            {
                //Unit on tile DOES NOT have a UnitController script. This should not happen.
                Debug.LogError("Unit on tile with GO ID: " + gameObject.GetInstanceID() 
                               + " has a unit with no UnitController script!");
            }
        }
    }

    public bool AttemptUnitMove(UnitController curUnit)
    {
        if (unitOnTile == null && curUnit.movementRange >= CalculateDist(curUnit.curCoords)) 
        {
            curUnit.transform.position = this.transform.position;
            GameController.instance.mapGrid[curUnit.curCoords.x, curUnit.curCoords.y].unitOnTile = null;
            unitOnTile = curUnit.gameObject;
            curUnit.curCoords = curCoords;

            curUnit.RotateUnitSprite();

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AttemptUnitAttack(UnitController curUnit, out int newHealth)
    {
        if (unitOnTile != null)
        {
            UnitController otherUnit = unitOnTile.GetComponent<UnitController>();

            if (curUnit.AttackRange >= CalculateDist(curUnit.curCoords) && !curUnit.OnSameTeam(otherUnit))
            {
                //otherUnit.unitHealth = Mathf.CeilToInt(((otherUnit.unitHealth * otherUnit.defenseStat) - (curUnit.unitHealth
                //                                    * curUnit.attackStat)) / (float)otherUnit.defenseStat);

                int damageAmt = curUnit.attackStat - otherUnit.defenseStat;
                Debug.Log("DamageAmt: " + damageAmt);
                if(damageAmt < 0)
                {
                    damageAmt = 0;
                }

                
                Vector3 dmgTextLoc = new Vector3(otherUnit.curCoords.x * 2f, 3, otherUnit.curCoords.y * 2f);

                GameObject newDmgIcon = GameObject.Instantiate(GameController.instance.damageAmtTextPrefab, GameController.instance.WorldSpaceCanvas.transform);
                newDmgIcon.transform.position = dmgTextLoc;
                Debug.Log(dmgTextLoc.ToString());
                newDmgIcon.GetComponent<Text>().text = "-" + damageAmt;
                newDmgIcon.GetComponent<FaceCamera>().DestroyInTime(2.5f);
                
                otherUnit.unitHealth -= damageAmt;

                newHealth = otherUnit.unitHealth;
                if (otherUnit.unitHealth <= 0)
                {
                    newHealth = 0;
                    GameController.instance.unitsInGame[otherUnit.unitTeamID].Remove(otherUnit);
                    Destroy(unitOnTile);
                    unitOnTile = null;

                    Vector3 koTextLoc = new Vector3(otherUnit.curCoords.x * 2f, 1.5f, otherUnit.curCoords.y * 2f);

                    GameObject newKOIcon = GameObject.Instantiate(GameController.instance.damageAmtTextPrefab, GameController.instance.WorldSpaceCanvas.transform);
                    newKOIcon.transform.position = koTextLoc;
                    Debug.Log(koTextLoc.ToString());
                    newKOIcon.GetComponent<Text>().text = "K.O.";
                    newKOIcon.GetComponent<FaceCamera>().DestroyInTime(2.5f);
                }
                return true;
            }
        }
        newHealth = 0;
        return false;
    }

    
    public int CalculateDist(IntVector2 otherCoords)
    {
        return (Mathf.Abs(curCoords.x - otherCoords.x) + Mathf.Abs(curCoords.y - otherCoords.y));
    }
}
