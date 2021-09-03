Imports deeplearning4vb.org.nd4j.linalg.api.ndarray

Friend Module RectangularArrays
    Public Function RectangularStringArray(ByVal size1 As Integer, ByVal size2 As Integer) As String()()
        Dim newArray As String()() = New String(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New String(size2 - 1) {}
        Next array1

        Return newArray
    End Function

    Public Function RectangularIntegerArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer) As Integer()()()
        Dim newArray As Integer()()() = New Integer(size1 - 1)()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Integer(size2 - 1)() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Integer(size3 - 1) {}
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularIntegerArray(ByVal size1 As Integer, ByVal size2 As Integer) As Integer()()
        Dim newArray As Integer()() = New Integer(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Integer(size2 - 1) {}
        Next array1

        Return newArray
    End Function

    Public Function RectangularDoubleArray(ByVal size1 As Integer, ByVal size2 As Integer) As Double()()
        Dim newArray As Double()() = New Double(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Double(size2 - 1) {}
        Next array1

        Return newArray
    End Function

    Public Function RectangularBooleanArray(ByVal size1 As Integer, ByVal size2 As Integer) As Boolean()()
        Dim newArray As Boolean()() = New Boolean(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Boolean(size2 - 1) {}
        Next array1

        Return newArray
    End Function

    Public Function RectangularSingleArray(ByVal size1 As Integer, ByVal size2 As Integer) As Single()()
        Dim newArray As Single()() = New Single(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Single(size2 - 1) {}
        Next array1

        Return newArray
    End Function

    Public Function RectangularSByteArray(ByVal size1 As Integer, ByVal size2 As Integer) As SByte()()
        Dim newArray As SByte()() = New SByte(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New SByte(size2 - 1) {}
        Next array1

        Return newArray
    End Function

    Public Function RectangularLongArray(ByVal size1 As Integer, ByVal size2 As Integer) As Long()()
        Dim newArray As Long()() = New Long(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Long(size2 - 1) {}
        Next array1

        Return newArray
    End Function

    Public Function RectangularINDArrayArray(ByVal size1 As Integer, ByVal size2 As Integer) As INDArray()()
        Dim newArray As INDArray()() = New INDArray(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New INDArray(size2 - 1) {}
        Next array1

        Return newArray
    End Function

    Public Function RectangularSByteArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer) As SByte()()()
        Dim newArray As SByte()()() = New SByte(size1 - 1)()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New SByte(size2 - 1)() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New SByte(size3 - 1) {}
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularSingleArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer) As Single()()()
        Dim newArray As Single()()() = New Single(size1 - 1)()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Single(size2 - 1)() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Single(size3 - 1) {}
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularSingleArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer, ByVal size4 As Integer) As Single()()()()
        Dim newArray As Single()()()() = New Single(size1 - 1)()()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Single(size2 - 1)()() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Single(size3 - 1)() {}
                    If size4 > -1 Then
                        For array3 As Integer = 0 To size3 - 1
                            newArray(array1)(array2)(array3) = New Single(size4 - 1) {}
                        Next array3
                    End If
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularDoubleArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer) As Double()()()
        Dim newArray As Double()()() = New Double(size1 - 1)()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Double(size2 - 1)() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Double(size3 - 1) {}
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularDoubleArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer, ByVal size4 As Integer) As Double()()()()
        Dim newArray As Double()()()() = New Double(size1 - 1)()()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Double(size2 - 1)()() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Double(size3 - 1)() {}
                    If size4 > -1 Then
                        For array3 As Integer = 0 To size3 - 1
                            newArray(array1)(array2)(array3) = New Double(size4 - 1) {}
                        Next array3
                    End If
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularObjectArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer, ByVal size4 As Integer, ByVal size5 As Integer) As Object()()()()()
        Dim newArray As Object()()()()() = New Object(size1 - 1)()()()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Object(size2 - 1)()()() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Object(size3 - 1)()() {}
                    If size4 > -1 Then
                        For array3 As Integer = 0 To size3 - 1
                            newArray(array1)(array2)(array3) = New Object(size4 - 1)() {}
                            If size5 > -1 Then
                                For array4 As Integer = 0 To size4 - 1
                                    newArray(array1)(array2)(array3)(array4) = New Object(size5 - 1) {}
                                Next array4
                            End If
                        Next array3
                    End If
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularCharArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer) As Char()()()
        Dim newArray As Char()()() = New Char(size1 - 1)()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Char(size2 - 1)() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Char(size3 - 1) {}
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularStringArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer) As String()()()
        Dim newArray As String()()() = New String(size1 - 1)()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New String(size2 - 1)() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New String(size3 - 1) {}
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularObjectArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer) As Object()()()
        Dim newArray As Object()()() = New Object(size1 - 1)()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Object(size2 - 1)() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Object(size3 - 1) {}
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularObjectArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer, ByVal size4 As Integer) As Object()()()()
        Dim newArray As Object()()()() = New Object(size1 - 1)()()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Object(size2 - 1)()() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Object(size3 - 1)() {}
                    If size4 > -1 Then
                        For array3 As Integer = 0 To size3 - 1
                            newArray(array1)(array2)(array3) = New Object(size4 - 1) {}
                        Next array3
                    End If
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularBooleanArray(ByVal size1 As Integer) As Boolean()
        Dim newArray As Boolean() = New Boolean(size1 - 1) {}

        Return newArray
    End Function

    Public Function RectangularSingleArray(ByVal size1 As Integer) As Single()
        Dim newArray As Single() = New Single(size1 - 1) {}

        Return newArray
    End Function

    Public Function RectangularDoubleArray(ByVal size1 As Integer) As Double()
        Dim newArray As Double() = New Double(size1 - 1) {}

        Return newArray
    End Function

    Public Function RectangularIntegerArray(ByVal size1 As Integer) As Integer()
        Dim newArray As Integer() = New Integer(size1 - 1) {}

        Return newArray
    End Function

    Public Function RectangularShortArray(ByVal size1 As Integer) As Short()
        Dim newArray As Short() = New Short(size1 - 1) {}

        Return newArray
    End Function

    Public Function RectangularSByteArray(ByVal size1 As Integer) As SByte()
        Dim newArray As SByte() = New SByte(size1 - 1) {}

        Return newArray
    End Function

    Public Function RectangularLongArray(ByVal size1 As Integer) As Long()
        Dim newArray As Long() = New Long(size1 - 1) {}

        Return newArray
    End Function

    Public Function RectangularLongArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer) As Long()()()
        Dim newArray As Long()()() = New Long(size1 - 1)()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Long(size2 - 1)() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Long(size3 - 1) {}
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularBooleanArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer) As Boolean()()()
        Dim newArray As Boolean()()() = New Boolean(size1 - 1)()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Boolean(size2 - 1)() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Boolean(size3 - 1) {}
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularObjectArray(ByVal size1 As Integer, ByVal size2 As Integer) As Object()()
        Dim newArray As Object()() = New Object(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Object(size2 - 1) {}
        Next array1

        Return newArray
    End Function

    Public Function RectangularShortArray(ByVal size1 As Integer, ByVal size2 As Integer) As Short()()
        Dim newArray As Short()() = New Short(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Short(size2 - 1) {}
        Next array1

        Return newArray
    End Function

    Public Function RectangularShortArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer) As Short()()()
        Dim newArray As Short()()() = New Short(size1 - 1)()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Short(size2 - 1)() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Short(size3 - 1) {}
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularLongArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer, ByVal size4 As Integer) As Long()()()()
        Dim newArray As Long()()()() = New Long(size1 - 1)()()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Long(size2 - 1)()() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Long(size3 - 1)() {}
                    If size4 > -1 Then
                        For array3 As Integer = 0 To size3 - 1
                            newArray(array1)(array2)(array3) = New Long(size4 - 1) {}
                        Next array3
                    End If
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularIntegerArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer, ByVal size4 As Integer) As Integer()()()()
        Dim newArray As Integer()()()() = New Integer(size1 - 1)()()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Integer(size2 - 1)()() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Integer(size3 - 1)() {}
                    If size4 > -1 Then
                        For array3 As Integer = 0 To size3 - 1
                            newArray(array1)(array2)(array3) = New Integer(size4 - 1) {}
                        Next array3
                    End If
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularShortArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer, ByVal size4 As Integer) As Short()()()()
        Dim newArray As Short()()()() = New Short(size1 - 1)()()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Short(size2 - 1)()() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Short(size3 - 1)() {}
                    If size4 > -1 Then
                        For array3 As Integer = 0 To size3 - 1
                            newArray(array1)(array2)(array3) = New Short(size4 - 1) {}
                        Next array3
                    End If
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularSByteArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer, ByVal size4 As Integer) As SByte()()()()
        Dim newArray As SByte()()()() = New SByte(size1 - 1)()()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New SByte(size2 - 1)()() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New SByte(size3 - 1)() {}
                    If size4 > -1 Then
                        For array3 As Integer = 0 To size3 - 1
                            newArray(array1)(array2)(array3) = New SByte(size4 - 1) {}
                        Next array3
                    End If
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularBooleanArray(ByVal size1 As Integer, ByVal size2 As Integer, ByVal size3 As Integer, ByVal size4 As Integer) As Boolean()()()()
        Dim newArray As Boolean()()()() = New Boolean(size1 - 1)()()() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Boolean(size2 - 1)()() {}
            If size3 > -1 Then
                For array2 As Integer = 0 To size2 - 1
                    newArray(array1)(array2) = New Boolean(size3 - 1)() {}
                    If size4 > -1 Then
                        For array3 As Integer = 0 To size3 - 1
                            newArray(array1)(array2)(array3) = New Boolean(size4 - 1) {}
                        Next array3
                    End If
                Next array2
            End If
        Next array1

        Return newArray
    End Function

    Public Function RectangularCharArray(ByVal size1 As Integer, ByVal size2 As Integer) As Char()()
        Dim newArray As Char()() = New Char(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Char(size2 - 1) {}
        Next array1

        Return newArray
    End Function
End Module