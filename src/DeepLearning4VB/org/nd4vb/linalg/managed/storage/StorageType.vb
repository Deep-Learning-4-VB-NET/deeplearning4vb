' DeepLearning4VB: element types of the managed storages (phase 2, see docs/PHASE2_DESIGN.md).

Imports System

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>
    ''' The element type of a <see cref="ManagedStorage"/>. The names are those of ND4J's <c>DataType</c>
    ''' (the managed data buffer maps one to the other by name); ND4J's <c>COMPRESSED</c> and
    ''' <c>UNKNOWN</c> have no storage and are not listed.
    ''' </summary>
    Public Enum StorageType
        ''' <summary>64-bit IEEE floating point, stored as <c>Double()</c>.</summary>
        [DOUBLE]
        ''' <summary>32-bit IEEE floating point, stored as <c>Single()</c>.</summary>
        FLOAT
        ''' <summary>16-bit IEEE half precision, stored as <c>UShort()</c> bit patterns.</summary>
        HALF
        ''' <summary>Signed 64-bit integer, stored as <c>Long()</c>.</summary>
        [LONG]
        ''' <summary>Signed 32-bit integer, stored as <c>Integer()</c>.</summary>
        INT
        ''' <summary>Signed 16-bit integer, stored as <c>Short()</c>.</summary>
        [SHORT]
        ''' <summary>Unsigned 8-bit integer, stored as <c>Byte()</c>.</summary>
        UBYTE
        ''' <summary>Signed 8-bit integer, stored as <c>SByte()</c>.</summary>
        [BYTE]
        ''' <summary>Boolean, stored as <c>Boolean()</c>; numerically 0 or 1.</summary>
        BOOL
        ''' <summary>Strings, stored as <c>String()</c>; no numeric access.</summary>
        UTF8
        ''' <summary>Brain floating point (the upper 16 bits of a FLOAT), stored as <c>UShort()</c> bit patterns.</summary>
        BFLOAT16
        ''' <summary>Unsigned 16-bit integer, stored as <c>UShort()</c>.</summary>
        UINT16
        ''' <summary>Unsigned 32-bit integer, stored as <c>UInteger()</c>.</summary>
        UINT32
        ''' <summary>Unsigned 64-bit integer, stored as <c>ULong()</c>.</summary>
        UINT64
    End Enum

    ''' <summary>Facts about <see cref="StorageType"/> values.</summary>
    Public NotInheritable Class StorageTypes

        Private Sub New()
        End Sub

        ''' <summary>True for DOUBLE, FLOAT, HALF and BFLOAT16.</summary>
        Public Shared Function IsFloatingPoint(ByVal type As StorageType) As Boolean
            Select Case type
                Case StorageType.DOUBLE, StorageType.FLOAT, StorageType.HALF, StorageType.BFLOAT16
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        ''' <summary>True for the signed and unsigned integer types (not BOOL).</summary>
        Public Shared Function IsInteger(ByVal type As StorageType) As Boolean
            Select Case type
                Case StorageType.LONG, StorageType.INT, StorageType.SHORT, StorageType.BYTE,
                     StorageType.UBYTE, StorageType.UINT16, StorageType.UINT32, StorageType.UINT64
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        ''' <summary>True for UBYTE, UINT16, UINT32 and UINT64.</summary>
        Public Shared Function IsUnsigned(ByVal type As StorageType) As Boolean
            Select Case type
                Case StorageType.UBYTE, StorageType.UINT16, StorageType.UINT32, StorageType.UINT64
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        ''' <summary>True for every type with numeric access (all but UTF8).</summary>
        Public Shared Function IsNumeric(ByVal type As StorageType) As Boolean
            Return type <> StorageType.UTF8
        End Function

        ''' <summary>Size of one element in bytes as ND4J counts it (UTF8 reports 0: strings have no fixed size).</summary>
        Public Shared Function ElementSize(ByVal type As StorageType) As Integer
            Select Case type
                Case StorageType.DOUBLE, StorageType.LONG, StorageType.UINT64
                    Return 8
                Case StorageType.FLOAT, StorageType.INT, StorageType.UINT32
                    Return 4
                Case StorageType.HALF, StorageType.BFLOAT16, StorageType.SHORT, StorageType.UINT16
                    Return 2
                Case StorageType.BYTE, StorageType.UBYTE, StorageType.BOOL
                    Return 1
                Case Else
                    Return 0
            End Select
        End Function

        ''' <summary>The .NET element type of the array that holds a storage of this type.</summary>
        Public Shared Function ElementType(ByVal type As StorageType) As Type
            Select Case type
                Case StorageType.DOUBLE
                    Return GetType(Double)
                Case StorageType.FLOAT
                    Return GetType(Single)
                Case StorageType.HALF, StorageType.BFLOAT16, StorageType.UINT16
                    Return GetType(UShort)
                Case StorageType.LONG
                    Return GetType(Long)
                Case StorageType.INT
                    Return GetType(Integer)
                Case StorageType.SHORT
                    Return GetType(Short)
                Case StorageType.UBYTE
                    Return GetType(Byte)
                Case StorageType.BYTE
                    Return GetType(SByte)
                Case StorageType.BOOL
                    Return GetType(Boolean)
                Case StorageType.UTF8
                    Return GetType(String)
                Case StorageType.UINT32
                    Return GetType(UInteger)
                Case StorageType.UINT64
                    Return GetType(ULong)
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(type))
            End Select
        End Function

    End Class

End Namespace
