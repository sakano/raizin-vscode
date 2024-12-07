namespace RaizinLanguageServer.Models.Definitions;

public interface IDescriptiveValue
{
    /// <summary>
    /// スクリプト中での値を取得します
    /// </summary>
    public string ScriptValue { get; }

    /// <summary>
    /// 検索用の値を取得します
    /// </summary>
    public string SearchValue { get; }

    /// <summary>
    /// 短い説明を取得します
    /// </summary>
    public string? ShortDescription { get; }

    /// <summary>
    /// 説明を取得します
    /// </summary>
    public string? Description { get; }
}