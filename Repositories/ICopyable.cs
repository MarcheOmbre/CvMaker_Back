namespace CvBuilderBack.Repositories;

public interface ICopyable<T>
{
    public void CopyFrom(T? other);

    public T CopyTo();
}