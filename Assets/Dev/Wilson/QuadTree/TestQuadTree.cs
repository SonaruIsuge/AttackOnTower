using System.Collections.Generic;
using UnityEngine;

namespace Dev.Wilson
{
    public class TestQuadTree : MonoBehaviour
    {
        public Rect       boundary;
        public int        maxObjects;
        public int        maxDepth;
        public int        spawnCount = 10;
        public TestEntity _prefab;
        public bool       IsQuadTree;

        private QuadTree quadTree;

        private GameObject _controledObj;
        private ICollider  _controledCollider;

        private List<ICollider> _allColliders     = new List<ICollider>();
        private List<ICollider> _collidersToCheck = new List<ICollider>();

        void Start()
        {
            quadTree = new QuadTree(boundary, maxObjects, maxDepth);

            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 position = new Vector2(Random.Range(boundary.xMin, boundary.xMax),
                    Random.Range(boundary.yMin, boundary.yMax));
                var        scale = Random.Range(.25f, .5f);
                GameObject obj   = Instantiate(_prefab.gameObject);
                obj.transform.position   = position;
                obj.transform.localScale = new Vector3(scale, scale, 1);
                _controledObj            = obj;

                obj.GetComponent<TestEntity>().SetId((ulong)i);

                var collider = obj.GetComponent<ICollider>();
                _controledCollider = collider;
                quadTree.Insert(collider);
                _allColliders.Add(collider);
            }
        }

        void Update()
        {
            Move();

            var objToCheckCollision = _controledObj;
            var colliderToCheck     = objToCheckCollision.GetComponent<ICollider>();
            _collidersToCheck.Clear();
            quadTree.GetCollidersInRange(colliderToCheck, quadTree.Root, _collidersToCheck);

            foreach (var collider in _allColliders)
            {
                collider.OnCollideExit();
            }

            if (IsQuadTree)
            {
                foreach (var collider in _collidersToCheck)
                {
                    if (collider != colliderToCheck && CheckCollision(colliderToCheck, collider))
                    {
                        collider.OnCollideEnter();
                    }
                }
            }
            else
            {
                foreach (var collider in _allColliders)
                {
                    if (collider != colliderToCheck && CheckCollision(colliderToCheck, collider))
                    {
                        collider.OnCollideEnter();
                    }
                }
            }
        }

        private bool CheckCollision(ICollider collider1, ICollider collider2)
        {
            var deltaX = collider1.X - collider2.X;
            var deltaY = collider1.Y - collider2.Y;
            var dis    = collider1.Radius + collider2.Radius;
            return deltaX * deltaX + deltaY * deltaY < dis * dis;
        }

        private void Move()
        {
            if (Input.GetKey(KeyCode.W))
            {
                OnObjectMove(_controledObj,
                    _controledObj.transform.position + new Vector3(0, 1, 0) * (3 * Time.deltaTime));
            }

            if (Input.GetKey(KeyCode.A))
            {
                OnObjectMove(_controledObj,
                    _controledObj.transform.position + new Vector3(-1, 0, 0) * (3 * Time.deltaTime));
            }

            if (Input.GetKey(KeyCode.S))
            {
                OnObjectMove(_controledObj,
                    _controledObj.transform.position + new Vector3(0, -1, 0) * (3 * Time.deltaTime));
            }

            if (Input.GetKey(KeyCode.D))
            {
                OnObjectMove(_controledObj,
                    _controledObj.transform.position + new Vector3(1, 0, 0) * (3 * Time.deltaTime));
            }
        }

        public void OnObjectMove(GameObject obj, Vector3 newPosition)
        {
            quadTree.Remove(_controledCollider);
            obj.transform.position = newPosition;
            quadTree.Insert(_controledCollider);
        }

        private void OnDrawGizmos()
        {
            if (quadTree == null)
                return;
            DrawQuadTree(quadTree.Root);
        }

        private void DrawQuadTree(QuadTreeNode node)
        {
            if (node == null)
                return;

            Gizmos.DrawWireCube(new Vector3(node.Boundary.center.x, node.Boundary.center.y, 0f),
                new Vector3(node.Boundary.width, node.Boundary.height, 0f));

            foreach (var child in node.Children)
            {
                DrawQuadTree(child);
            }
        }
    }
}