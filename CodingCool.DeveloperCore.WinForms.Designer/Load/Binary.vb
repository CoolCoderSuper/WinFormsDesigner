Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Serialization

Namespace Load
    Friend Module Binary
        ''' <summary>
        ''' Convert an object to a Byte Array.
        ''' </summary>
        Public Function ObjectToByteArray(objData As Object) As Byte()
            If objData Is Nothing Then Return Nothing

            Return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(objData, GetJsonSerializerOptions()))
        End Function

        ''' <summary>
        ''' Convert a byte array to an Object of T.
        ''' </summary>
        Public Function ByteArrayToObject(Of T)(byteArray As Byte()) As T
            If byteArray Is Nothing OrElse Not byteArray.Any() Then Return Nothing
            Return JsonSerializer.Deserialize(Of T)(byteArray, GetJsonSerializerOptions())
        End Function

        ''' <summary>
        ''' Convert a byte array to an Object.
        ''' </summary>
        Public Function ByteArrayToObject(byteArray As Byte(), returnType As Type) As Object
            If byteArray Is Nothing OrElse Not byteArray.Any() Then Return Nothing
            Return JsonSerializer.Deserialize(byteArray, returnType, GetJsonSerializerOptions())
        End Function
        
        Private Function GetJsonSerializerOptions() As JsonSerializerOptions
            Return New JsonSerializerOptions() With {
                .PropertyNamingPolicy = Nothing,
                .WriteIndented = True,
                .AllowTrailingCommas = True,
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }
        End Function
    End Module
End NameSpace