using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class SlicePan : MonoBehaviour
{
    [SerializeField] private Material objMat;
    [SerializeField] private LayerMask objMask;
    [SerializeField] private GameObject objWheat;
    [SerializeField] private GameObject objWheatBlock;

    private int curHpWheat = 100;
    private int maxpWheat = 100;
    private int charDgm = 50;
    private bool isDamaged = false;
    private bool isStart = false;

    public void OnDrawGizmos()
    {
        EzySlice.Plane cuttingPlane = new EzySlice.Plane();

        cuttingPlane.Compute(transform);

        cuttingPlane.OnDebugDraw();
    }

    private void Start()
    {
        Instantiate(objWheat, transform.position, Quaternion.Euler(-90, 0, 0), transform);
    }

    public SlicedHull Slicer(GameObject obj, Material mat = null)
    {
        if (!isDamaged)
        {
            return obj.Slice(transform.position + new Vector3(0, 0.5f, 0), transform.up, mat);
        }
        else
        {
            return obj.Slice(transform.position, transform.up, mat);
        }
    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.transform.GetChild(1).GetChild(2).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).gameObject.CompareTag("Scythe") && !isStart)
        {
            isStart = true;
            StartCoroutine(WheatDmg());
        }
    }

    private IEnumerator WheatDmg()
    {
        if (curHpWheat > maxpWheat / 2 + 1)
        {
            curHpWheat -= charDgm;
            Collider[] slicedObjs = Physics.OverlapBox(transform.position, new Vector3(1, 0.1f, 0.1f), transform.rotation, objMask);

            foreach (Collider slicedObj in slicedObjs)
            {
                SlicedHull slicedHull = Slicer(slicedObj.GetComponent<Collider>().gameObject, objMat);
                GameObject objUp = slicedHull.CreateUpperHull(slicedObj.gameObject, objMat);
                GameObject objDown = slicedHull.CreateLowerHull(slicedObj.gameObject, objMat);

                objUp.AddComponent<MeshCollider>().convex = true;
                objDown.AddComponent<MeshCollider>().convex = true;

                objUp.transform.parent = transform;
                objUp.transform.localPosition = Vector3.zero;
                objDown.layer = 6;
                objDown.tag = "Wheat";
                objDown.transform.parent = transform;
                objDown.transform.localPosition = Vector3.zero;

                objUp.AddComponent<Rigidbody>();
                objUp.GetComponent<Rigidbody>().AddExplosionForce(200, objUp.transform.position + new Vector3(Random.Range(-1, 1), -1, Random.Range(-1, 1)), 5);
                Destroy(objUp, 2);

                Destroy(slicedObj.gameObject);

                GameObject wheatBlock = Instantiate(objWheatBlock, transform.position, Quaternion.identity) as GameObject;
                wheatBlock.GetComponent<Rigidbody>().AddExplosionForce(200, wheatBlock.transform.position + new Vector3(Random.Range(-1, 1), -1, Random.Range(-1, 1)), 1);
                Destroy(wheatBlock.GetComponent<Rigidbody>(), 2);
                yield return new WaitForSeconds(2);
                wheatBlock.GetComponent<BoxCollider>().isTrigger = true;
                wheatBlock.GetComponent<BoxCollider>().size = new Vector3(5, 20, 10);
            }

            isDamaged = true;
        }
        else
        {
            Collider[] slicedObjs = Physics.OverlapBox(transform.position, new Vector3(1, 0.1f, 0.1f), transform.rotation, objMask);

            foreach (Collider slicedObj in slicedObjs)
            {
                SlicedHull slicedHull = Slicer(slicedObj.GetComponent<Collider>().gameObject, objMat);
                GameObject objUp = slicedHull.CreateUpperHull(slicedObj.gameObject, objMat);
                GameObject objDown = slicedHull.CreateLowerHull(slicedObj.gameObject, objMat);

                objUp.AddComponent<MeshCollider>().convex = true;
                objDown.AddComponent<MeshCollider>().convex = true;

                objUp.transform.parent = transform;
                objUp.transform.localPosition = Vector3.zero;
                objDown.layer = 6;
                objDown.tag = "Wheat";
                objDown.transform.parent = transform;
                objDown.transform.localPosition = Vector3.zero;

                objUp.AddComponent<Rigidbody>();
                objUp.GetComponent<Rigidbody>().AddExplosionForce(5, objUp.transform.position + new Vector3(Random.Range(-1, 1), -1, Random.Range(-1, 1)), 5);
                Destroy(objUp, 2);

                Destroy(objDown.gameObject, 12);
                Destroy(slicedObj.gameObject);

                GameObject wheatBlock = Instantiate(objWheatBlock, transform.position, Quaternion.identity) as GameObject;
                wheatBlock.GetComponent<Rigidbody>().AddExplosionForce(200, wheatBlock.transform.position + new Vector3(Random.Range(-1, 1), -1, Random.Range(-1, 1)), 1);
                Destroy(wheatBlock.GetComponent<Rigidbody>(), 2);
                yield return new WaitForSeconds(2);
                wheatBlock.GetComponent<BoxCollider>().isTrigger = true;
                wheatBlock.GetComponent<BoxCollider>().size = new Vector3(5, 20, 10);
            }

            yield return new WaitForSeconds(10);
            curHpWheat = 100;
            Instantiate(objWheat, transform.position, Quaternion.Euler(-90, 0, 0), transform);

            isDamaged = false;
        }

        yield return new WaitForSeconds(1);
        isStart = false;
    }
}
