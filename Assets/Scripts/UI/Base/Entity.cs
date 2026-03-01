using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    public object Model { get; protected set; }

    protected bool IsInitialRefresh { get; private set; } = true;

    private float _tickInterval = 0f;
    private float _tickTimer = 0f;

    protected EntityBase SetModel(object model, bool activate = true)
    {
        if (IsInitialRefresh)
            InitialPrepare();

        Model = model;

        Prepare();
        Refresh();

        IsInitialRefresh = false;

        if (activate) gameObject.SetActive(true);

        return this;
    }

    public virtual void InitialPrepare() { }

    public virtual void Prepare() { }

    public abstract void Refresh();

    public virtual void Tick() { }

    public void SetTickInterval(float interval)
    {
        _tickInterval = interval;
    }

    protected virtual void Update()
    {
        _tickTimer += Time.deltaTime;
        if (_tickTimer >= _tickInterval)
        {
            _tickTimer = 0f;
            Tick();
        }
    }
}


public abstract class Entity<T> : EntityBase
{
    public new T Model => (T)base.Model;

    public Entity<T> SetModel(T model, bool activate = true)
    {
        base.SetModel(model, activate);

        return this;
    }
}