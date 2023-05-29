using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskState {

    Inactive,
    Running,
    Complete
}

[CreateAssetMenu(menuName = "Quest/Task/Task", fileName = "Task_")]
public class Task : ScriptableObject {

    #region Events
    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);
    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    #endregion

    [SerializeField]
    private Category category;
    [Header("Text")]
    [SerializeField]
    private string codeName;
    [SerializeField]
    private string description;

    [Header("Action")]
    [SerializeField]
    private TaskAction action;

    [Header("Target")]
    [SerializeField]
    private TaskTarget[] targets;

    [Header("Setting")]
    [SerializeField]
    private InitialSuccessValue initialSuccessValue;
    [SerializeField]
    private int needSuccessToComplete;

    private TaskState state;

    public event StateChangedHandler onStateChanged;
    public event SuccessChangedHandler onSuccessChanged;

    public int CurrentSuccess { get; private set; }
    public Category Category => category;
    public string CodeName => codeName;
    public string Description => description;
    public int NeedSuccessToComplete => needSuccessToComplete;

    public TaskState State
    {
        get => state;
        set
        {
            var prevState = state;
            state = value;
            onStateChanged?.Invoke(this, state, prevState);
        }
    }

    public bool IsComplete => State == TaskState.Complete;

    public void ReceiveReport(int successCount) {
        CurrentSuccess = action.Run(this, CurrentSuccess, successCount);
    }

    //�� �Լ��� TaskTarget�� ���� �� Task�� ���� Ƚ���� ���� ���� ������� Ȯ���ϴ� �Լ� �Դϴ�.
    //Setting�س��� Target�� �߿� �ش��ϴ� Target�� ������ true��, ������ false�� ��ȯ�մϴ�.
    public bool IsTarget(string category, object target)
        => Category == Category &&
        targets.Any(x => x.IsEqual(target)) &&
        !IsComplete;
}
