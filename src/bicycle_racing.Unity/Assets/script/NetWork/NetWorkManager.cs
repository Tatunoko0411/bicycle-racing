using DG.Tweening;
using bicycle_racing.Shared.Interfaces.StreamingHubs;
using bicycle_racing.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class NetWorkManager : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;
    [SerializeField] InputField roomNameField;
    [SerializeField] InputField PlayerIdField;
    [SerializeField] Text info;
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    RoomModel roomModel;
    UserModel userModel;

    int myUserId = 7;
    User myself;

    float waitTime = 0;

    bool isJoin = false;

    [SerializeField]PlayerManager playerManager;

    async void Start()
    {
        roomModel = GetComponent<RoomModel>();
        userModel = GetComponent<UserModel>();

        //ユーザーが入室した時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMoveUser += this.OnMoveUser;
        //接続
        await roomModel.ConnectAsync();

        try
        {
            // ユーザー情報を取得
            myself = await userModel.GetUser(myUserId);

            Debug.Log("取得成功");
        }
        catch (Exception e)
        {
            Debug.Log("RegistUser failed");
            Debug.LogException(e);
        }

    }

    private void FixedUpdate()
    {
        if (isJoin)
        {
            waitTime++;

            if (waitTime >= 5)
            {
                Move();
                waitTime = 0;
            }
        }
    }


    public async void JoinRoom()
    {
        int ID = int.Parse(PlayerIdField.text);
        myUserId = ID;
        if (roomNameField.text == "")
        {
            return;
        }

        if(ID != myUserId)
        {
            User user =  await userModel.GetUser(ID);
            if (user == null)
            {
                Debug.Log("ユーザーがありませんでした");
                return;
            }
        }

        //入室
        Debug.Log("入室処理開始");
        await roomModel.JoinAsync(roomNameField.text,ID);
    }

    public async void LeaveRoom()
    {
        await roomModel.LeaveAsync();
        isJoin = false;
    }

    public async void Move()
    {
        await roomModel.MoveAsync(playerManager.gameObject.transform.position, playerManager.gameObject.transform.rotation);
    }
    //ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        // すでに表示済みのユーザーは追加しない
        if (characterList.ContainsKey(user.ConnectionId))
        {
            return;
        }

        // 自分は追加しない
        if (user.UserData.Id == myUserId)
        {
            info.text = ($"接続ID：{user.ConnectionId} " +
               $"ユーザーID:{user.UserData.Id}" +
               $" ユーザー名：{user.UserData.Name}");

            isJoin = true;
            return;
        }

        GameObject characterObject = Instantiate(characterPrefab);  //インスタンス生成
        characterObject.transform.position = new Vector3(0, 0, 0);
        characterList[user.ConnectionId] = characterObject;  //フィールドで保持
    }

    //ユーザーが退室した時の処理
    private void OnLeavedUser(JoinedUser user)
    {
        if(user.UserData.Id == myUserId)
        {
            return;
        }

        Destroy(characterList[user.ConnectionId]);
        characterList[user.ConnectionId] = null;

        Debug.Log("削除");
    }

    // 自分以外のユーザーの移動を反映
    private void OnMoveUser(Guid connectionId, Vector3 pos, Quaternion rot)
    {
        // いない人は移動できない
        if (!characterList.ContainsKey(connectionId))
        {
            Debug.Log("いないよぉ！");
            return;
        }

        Debug.Log($"{pos}に動くよ！{connectionId}が！");

        // DOTweenを使うことでなめらかに動く！
        characterList[connectionId].transform.DOMove(pos, 0.1f);
        characterList[connectionId].transform.DORotate(new Vector3(rot.x,rot.y,rot.z), 0.1f);
    
    }

}
