using UnityEngine;
using System.Collections.Generic;

public class TDCharacterManager : MonoBehaviour
{
    private Stack<IAction> undoStack = new Stack<IAction>();
    private Stack<IAction> redoStack = new Stack<IAction>();

    private TDCharacterController controller;
    private TDColorChanger colorChanger;

    private void Start()
    {
        controller = FindObjectOfType<TDCharacterController>();
        colorChanger = FindObjectOfType<TDColorChanger>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) 
            Undo();

        if (Input.GetKeyDown(KeyCode.Y)) 
            Redo();

        if (Input.GetKeyDown(KeyCode.R)) 
            colorChanger.ChangeToRandomColor();
    }


    public void RegisterMove(Vector3 oldPos, Vector3 newPos)
    {
        IAction action = new MoveAction(controller.gameObject, oldPos, newPos);
        undoStack.Push(action);
        redoStack.Clear();
    }

    public void RegisterColorChange(Color oldColor, Color newColor)
    {
        IAction action = new ColorAction(colorChanger, oldColor, newColor);
        undoStack.Push(action);
        redoStack.Clear();
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            IAction action = undoStack.Pop();
            action.Undo();
            redoStack.Push(action);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            IAction action = redoStack.Pop();
            action.Redo();
            undoStack.Push(action);
        }
    }

    private interface IAction
    {
        void Undo();
        void Redo();
    }

    private class MoveAction : IAction
    {
        private GameObject obj;
        private Vector3 oldPos, newPos;

        public MoveAction(GameObject obj, Vector3 oldPos, Vector3 newPos)
        {
            this.obj = obj;
            this.oldPos = oldPos;
            this.newPos = newPos;
        }

        public void Undo() => obj.transform.position = oldPos;
        public void Redo() => obj.transform.position = newPos;
    }

    private class ColorAction : IAction
    {
        private TDColorChanger changer;
        private Color oldColor, newColor;

        public ColorAction(TDColorChanger changer, Color oldColor, Color newColor)
        {
            this.changer = changer;
            this.oldColor = oldColor;
            this.newColor = newColor;
        }

        public void Undo() => changer.SetColor(oldColor);
        public void Redo() => changer.SetColor(newColor); 
    }
}
