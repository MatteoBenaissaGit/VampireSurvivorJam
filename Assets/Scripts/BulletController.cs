public class BulletController : Damager
{
    private void Start()
    {
        base.Start();

        Destroy(gameObject,10);
    }
}
