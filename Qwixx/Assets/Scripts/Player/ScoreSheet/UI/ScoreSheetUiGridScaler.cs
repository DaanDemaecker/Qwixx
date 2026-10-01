using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ScoreSheetUiGridScaler : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rect = null;

    [SerializeField]
    private GridLayoutGroup _grid = null;

    [SerializeField]
    private int _rowAmount = 2;

    private int _childAmount = 0;

    private void OnValidate()
    {
        CalculateGridSize();
    }

    private void OnRectTransformDimensionsChange()
    {
        CalculateGridSize();
    }

    private void LateUpdate()
    {
        if(_childAmount != transform.childCount)
        {
            _childAmount = transform.childCount;
            CalculateGridSize();
        }
    }
    private void CalculateGridSize()
    {
        if(_rect == null || _grid == null || _rowAmount == 0 || transform.childCount == 0)
        {
            return;
        }

        _grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        _grid.constraintCount = _rowAmount;


        int childAmount = transform.childCount;
        float columnAmountFloat = (float)childAmount / (float)_rowAmount;
        int columnAmountInt = (int)columnAmountFloat;
        if(columnAmountFloat > columnAmountInt)
        {
            columnAmountInt++;
        }

        Vector2 size = _rect.rect.size;

        float gridSizeY = (size.y - (_grid.spacing.y * (_rowAmount - 1))  - _grid.padding.top - _grid.padding.bottom)/_rowAmount;

        float gridSizeX = (size.x - (_grid.spacing.x * (columnAmountInt - 1)) - _grid.padding.left - _grid.padding.right)/ columnAmountInt;


        _grid.cellSize = new Vector2(gridSizeX, gridSizeY);
    }
}
