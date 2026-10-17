'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports DeepLearning4VB.org.nd4vb.common.function
Imports AggregableColumnReduction = DeepLearning4VB.org.datavec.api.transform.reduce.AggregableColumnReduction
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports StringMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.StringMetaData
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.reduce.impl

    ''' <summary>
    ''' A custom column reduction that calculates the geographic midpoint (centre of gravity) of a set of
    ''' "latitude,longitude" coordinates given as text (decimal degrees), with the delimiter given to the constructor.
    ''' The output is the midpoint in the same text format (Java <c>Double.toString</c> formatting). See
    ''' <a href="http://www.geomidpoint.com/example.html">http://www.geomidpoint.com/example.html</a>.
    ''' Coordinates are parsed with the invariant culture. Equality (Lombok <c>@Data</c>) compares the delimiter and
    ''' the new column name.
    ''' </summary>
    <Serializable>
    Public Class GeographicMidpointReduction
        Implements AggregableColumnReduction

        Public Const EDGE_CASE_EPS As Double = 0.000000001

        Private delim As String
        Private newColumnName As String

        ''' <param name="delim"> Delimiter for the coordinates in text format. For example, if format is "lat,long" use "," </param>
        Public Sub New(ByVal delim As String)
            Me.New(delim, Nothing)
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal delim As String, ByVal newColumnName As String)
            Me.delim = delim
            Me.newColumnName = newColumnName
        End Sub

        ''' <summary>Java <c>getDelim()</c> (Lombok).</summary>
        Public Function getDelim() As String
            Return delim
        End Function

        ''' <summary>Java <c>setDelim()</c> (Lombok).</summary>
        Public Sub setDelim(ByVal delim As String)
            Me.delim = delim
        End Sub

        ''' <summary>Java <c>getNewColumnName()</c> (Lombok).</summary>
        Public Function getNewColumnName() As String
            Return newColumnName
        End Function

        ''' <summary>Java <c>setNewColumnName()</c> (Lombok).</summary>
        Public Sub setNewColumnName(ByVal newColumnName As String)
            Me.newColumnName = newColumnName
        End Sub

        Public Overridable Function reduceOp() As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) Implements AggregableColumnReduction.reduceOp
            Return New AverageCoordinateReduceOp(delim)
        End Function

        Public Overridable Function getColumnsOutputName(ByVal columnInputName As String) As IList(Of String) Implements AggregableColumnReduction.getColumnsOutputName
            If newColumnName IsNot Nothing Then
                Return JavaCollections.singletonList(newColumnName)
            End If
            Return JavaCollections.singletonList("midpoint(" & columnInputName & ")")
        End Function

        Public Overridable Function getColumnOutputMetaData(ByVal newColumnName As IList(Of String), ByVal columnInputMeta As ColumnMetaData) As IList(Of ColumnMetaData) Implements AggregableColumnReduction.getColumnOutputMetaData
            Return JavaCollections.singletonList(Of ColumnMetaData)(New StringMetaData(newColumnName(0)))
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            'No change
            Return inputSchema
        End Function

        ''' <summary>Not used by this reduction: setting it is a no-op and getting it returns Nothing.</summary>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Get
                Return Nothing
            End Get
            Set(ByVal value As SchemaT)
                'No op
            End Set
        End Property

        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return Nothing
        End Function

        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return New String() {}
        End Function

        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New String() {}
        End Function

        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return Nothing
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As GeographicMidpointReduction = DirectCast(obj, GeographicMidpointReduction)
            Return String.Equals(delim, other.delim) AndAlso String.Equals(newColumnName, other.newColumnName)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(delim, newColumnName)
        End Function

        Public Overrides Function ToString() As String
            Return "GeographicMidpointReduction(delim=" & delim & ", newColumnName=" & Objects.toString(newColumnName) & ")"
        End Function

        ''' <summary>
        ''' The accumulator: sums the unit vectors (x, y, z) of the coordinates; the result is the coordinate of the
        ''' mean vector. Combining adds the sums of the other accumulator.
        ''' </summary>
        <Serializable>
        Public Class AverageCoordinateReduceOp
            Implements IAggregableReduceOp(Of WritableT, IList(Of WritableT))

            Friend Shared ReadOnly PI_180 As Double = Math.PI / 180.0

            Friend delim As String

            Friend sumx As Double
            Friend sumy As Double
            Friend sumz As Double
            Friend count As Integer

            Public Sub New(ByVal delim As String)
                Me.delim = delim
            End Sub

            Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of WritableT, IList(Of WritableT))) Implements IAggregableReduceOp(Of WritableT, IList(Of WritableT)).combine
                Dim r As AverageCoordinateReduceOp = TryCast(accu, AverageCoordinateReduceOp)
                If r IsNot Nothing Then
                    sumx += r.sumx
                    sumy += r.sumy
                    sumz += r.sumz
                    count += r.count
                Else
                    Throw New InvalidOperationException("Cannot combine type of class: " & If(accu Is Nothing, "null", "class " & accu.GetType().FullName))
                End If
            End Sub

            Public Overridable Sub accept(ByVal writable As WritableT) Implements Consumer(Of WritableT).accept
                Dim str As String = writable.ToString()
                ' Java String.split(delim): the delimiter is a regular expression and trailing empty strings are removed
                Dim split() As String = StringHelper.Split(str, delim, True)
                If split.Length <> 2 Then
                    Throw New InvalidOperationException("Could not parse lat/long string: """ & str & """")
                End If
                Dim latDeg As Double = JavaNumbers.parseDouble(split(0))
                Dim longDeg As Double = JavaNumbers.parseDouble(split(1))

                ' The Java code checks the latitude twice (the second check was meant for the longitude); kept as is
                Preconditions.checkState(latDeg >= -90.0 AndAlso latDeg <= 90.0, "Invalid latitude: must be -90 to -90. Got: %s", latDeg)
                Preconditions.checkState(latDeg >= -180.0 AndAlso latDeg <= 180.0, "Invalid longitude: must be -180 to -180. Got: %s", longDeg)

                Dim lat As Double = latDeg * PI_180
                Dim lng As Double = longDeg * PI_180

                Dim x As Double = Math.Cos(lat) * Math.Cos(lng)
                Dim y As Double = Math.Cos(lat) * Math.Sin(lng)
                Dim z As Double = Math.Sin(lat)

                sumx += x
                sumy += y
                sumz += z
                count += 1
            End Sub

            Public Overridable Function [get]() As IList(Of WritableT) Implements Supplier(Of IList(Of WritableT)).get
                Dim x As Double = sumx / count
                Dim y As Double = sumy / count
                Dim z As Double = sumz / count

                If count = 0 Then
                    Throw New InvalidOperationException("Cannot calculate geographic midpoint: no datapoints were added to be reduced")
                End If

                If Math.Abs(x) < EDGE_CASE_EPS AndAlso Math.Abs(y) < EDGE_CASE_EPS AndAlso Math.Abs(z) < EDGE_CASE_EPS Then
                    Throw New InvalidOperationException("No Geographic midpoint exists: midpoint is center of the earth")
                End If

                Dim longRad As Double = Math.Atan2(y, x)
                Dim hyp As Double = Math.Sqrt(x * x + y * y)
                Dim latRad As Double = Math.Atan2(z, hyp)

                Dim latDeg As Double = latRad / PI_180
                Dim longDeg As Double = longRad / PI_180

                Preconditions.checkState(Not Double.IsNaN(latDeg), "Final latitude is NaN")
                Preconditions.checkState(Not Double.IsNaN(longDeg), "Final longitude is NaN")

                Dim str As String = Arrays.JavaDoubleToString(latDeg) & delim & Arrays.JavaDoubleToString(longDeg)
                Return JavaCollections.singletonList(Of WritableT)(New Text(str))
            End Function
        End Class
    End Class

End Namespace
