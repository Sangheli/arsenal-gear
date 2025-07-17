using System.Collections;
using UnityEngine;
using System;

public class ArcMoveAnimator : MonoBehaviour
{
    public int enemyCount = 100; // сколько врагов спавнить
    public int delim = 3;
    public float duration = 1.5f;
    public float startScale = 2.5f;
    public float arcHeight = 2f;
    public float edgeRadius = 8f;
    public float delayBetween = 0.15f;
    public float minDelay = 0.02f; // минимальная задержка
    private Vector3 _centerScreen;

    public void Play()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        _centerScreen = Camera.main.ScreenToWorldPoint(screenCenter);
        _centerScreen.z = 0f;
        StartCoroutine(AnimateAll());
    }

    IEnumerator AnimateAll()
    {
        var sortingOrderData= new SortingOrderData { ramka = 4, Start = 4, Max = 15000 };
        int finished = 0;
        
        var enemyList = G.I.Data.enemyContainer.List; ;

        for (int i = 0; i < enemyCount; i++)
        {
            var enemyData = enemyList[UnityEngine.Random.Range(0, enemyList.Count)].prefab;
            var enemy = G.I.ObjectPool.Get(enemyData, GetRandomEdgePosition());
            enemy.ResetView();
            enemy.transform.localScale = Vector3.one * startScale;
            enemy.view.SetSortingOrder(sortingOrderData);
            sortingOrderData.Iterrate();
            
            if (i % delim == 0)
            {
                Debug.Log(i);
                G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.loadinScreen);
            }

            StartCoroutine(AnimateObject(enemy.transform, () =>
            {
                finished++;
                G.I.ObjectPool.Return(enemy);
                if (finished == enemyCount) G.I.StartRun(false);
            }));
            // вычисляем текущую задержку: t от 0 до 1
            float t = (float)i / Mathf.Max(1, (enemyCount - 1));
            float currentDelay = Mathf.Lerp(delayBetween, minDelay, t);
            yield return new WaitForSeconds(currentDelay);
        }
    }

    IEnumerator AnimateObject(Transform obj, Action onComplete)
    {
        Vector3 start = obj.position;
        Vector3 end = _centerScreen;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Vector3 pos = Vector3.Lerp(start, end, t);
            // Дуга в 2D: смещаем по нормали к направлению движения
            Vector3 dir = (end - start).normalized;
            Vector3 normal = new Vector3(-dir.y, dir.x, 0f); // нормаль в 2D
            float arc = Mathf.Sin(Mathf.PI * t) * arcHeight;
            pos += normal * arc;
            obj.position = pos;
            obj.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.zero, t);
            yield return null;
        }
        obj.localScale = Vector3.zero;
        obj.position = end;
        onComplete?.Invoke();
    }

    Vector3 GetRandomEdgePosition()
    {
        // Случайная точка на окружности вокруг центра экрана
        float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
        Vector3 pos = _centerScreen + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * edgeRadius;
        pos.z = 0;
        return pos;
    }
} 