using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static Define;
using Object = UnityEngine.Object;

public class UI_TitleScene : UI_Scene
{
	enum GameObjects
    {
        BG,
    }

    enum Texts
    {
        StartText,
    }

	private enum TitleSceneState
	{
		None,
		AssetLoading,
		AssetLoaded,
		ConnectingToServer,
		ConnectedToServer,
		FailedToConnectToServer,
	}

	TitleSceneState _state = TitleSceneState.None;
	TitleSceneState State
	{
		get { return _state; }
		set
		{
			_state = value;
			switch (value)
			{
				case TitleSceneState.None:
					break;
				case TitleSceneState.AssetLoading:
					GetText((int)Texts.StartText).text = $"TODO AssetLoading";
					break;
				case TitleSceneState.AssetLoaded:
					GetText((int)Texts.StartText).text = "TODO AssetLoaded";
					break;
				case TitleSceneState.ConnectingToServer:
					GetText((int)Texts.StartText).text = "TODO ���� ������";
					break;
				case TitleSceneState.ConnectedToServer:
					GetText((int)Texts.StartText).text = "Tap To Start";
					break;
				case TitleSceneState.FailedToConnectToServer:
					GetText((int)Texts.StartText).text = "TODO ���� ���� ����";
					break;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		BindObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));

		GetObject((int)GameObjects.BG).BindEvent((evt) =>
		{
			Debug.Log("OnClick");
			Managers.Scene.LoadScene(EScene.SelectStageScene);
		});

		GetObject((int)GameObjects.BG).gameObject.SetActive(false);
	}

	protected override void Start()
	{
		base.Start();

		// Load ����
		State = TitleSceneState.AssetLoading;

		Managers.Resource.LoadAllAsync<Object>("Preload", (key, count, totalCount) =>
		{
			GetText((int)Texts.StartText).text = $"TODO Load : {key} {count}/{totalCount}";
			//Debug.Log($"TODO Load : {key} {count}/{totalCount}");

			if (count == totalCount)
			{
				OnAssetLoaded();
			}
		});
	}

	private void OnAssetLoaded()
	{
		State = TitleSceneState.AssetLoaded;
		Managers.Data.Init();

		// TODO ILHAK 나중에 서버 접속 필요하면 구현
		// Debug.Log("Connecting To Server");
		// State = TitleSceneState.ConnectingToServer;

		// IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
		// IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		// Managers.Network.GameServer.Connect(endPoint, OnConnectionSuccess, OnConnectionFailed);
		OnConnectionSuccess();
	}

	private void OnConnectionSuccess()
	{
		Debug.Log("Connected To Server");
		State = TitleSceneState.ConnectedToServer;
		Managers.Sound.Play(Define.ESound.Effect, "Sound_Opening");  

		GetObject((int)GameObjects.BG).gameObject.SetActive(true);

		//StartCoroutine(CoSendTestPackets());
	}

	private void OnConnectionFailed()
	{
		Debug.Log("Failed To Connect To Server");
		State = TitleSceneState.FailedToConnectToServer;
	}

	IEnumerator CoSendTestPackets()
	{
		while (true)
		{
			yield return new WaitForSeconds(1);

			C_Test pkt = new C_Test();
			pkt.Temp = 1;
			Managers.Network.Send(pkt);
		}
	}
}
