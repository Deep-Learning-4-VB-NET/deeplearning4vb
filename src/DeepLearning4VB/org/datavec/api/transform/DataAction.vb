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
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports FilterT = DeepLearning4VB.org.datavec.api.transform.filter.Filter
Imports CalculateSortedRankT = DeepLearning4VB.org.datavec.api.transform.rank.CalculateSortedRank
Imports IAssociativeReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports ConvertFromSequenceT = DeepLearning4VB.org.datavec.api.transform.sequence.ConvertFromSequence
Imports ConvertToSequenceT = DeepLearning4VB.org.datavec.api.transform.sequence.ConvertToSequence
Imports SequenceSplitT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceSplit

Namespace org.datavec.api.transform

    ''' <summary>
    ''' A single step of a <see cref="TransformProcess"/>: exactly one of a transform, a filter, a conversion to or from
    ''' sequences, a sequence split, a reduction or a sorted-rank calculation is set. Equality (Lombok <c>@Data</c>)
    ''' compares all seven fields; JSON omits the null ones (<c>@JsonInclude(NON_NULL)</c>).
    ''' </summary>
    <Serializable>
    <JsonIncludeNonNull>
    Public Class DataAction

        Private transformField As Transform
        Private filterField As FilterT
        Private convertToSequenceField As ConvertToSequenceT
        Private convertFromSequenceField As ConvertFromSequenceT
        Private sequenceSplitField As SequenceSplitT
        Private reducerField As IAssociativeReducerT
        Private calculateSortedRankField As CalculateSortedRankT

        Public Sub New()
            'No-arg constructor for Jackson
        End Sub

        Public Sub New(ByVal transform As Transform)
            Me.transformField = transform
        End Sub

        Public Sub New(ByVal filter As FilterT)
            Me.filterField = filter
        End Sub

        Public Sub New(ByVal convertToSequence As ConvertToSequenceT)
            Me.convertToSequenceField = convertToSequence
        End Sub

        Public Sub New(ByVal convertFromSequence As ConvertFromSequenceT)
            Me.convertFromSequenceField = convertFromSequence
        End Sub

        Public Sub New(ByVal sequenceSplit As SequenceSplitT)
            Me.sequenceSplitField = sequenceSplit
        End Sub

        Public Sub New(ByVal reducer As IAssociativeReducerT)
            Me.reducerField = reducer
        End Sub

        Public Sub New(ByVal calculateSortedRank As CalculateSortedRankT)
            Me.calculateSortedRankField = calculateSortedRank
        End Sub

        ' ---- Lombok @Data accessors ----

        Public Function getTransform() As Transform
            Return transformField
        End Function

        Public Sub setTransform(ByVal transform As Transform)
            Me.transformField = transform
        End Sub

        Public Function getFilter() As FilterT
            Return filterField
        End Function

        Public Sub setFilter(ByVal filter As FilterT)
            Me.filterField = filter
        End Sub

        Public Function getConvertToSequence() As ConvertToSequenceT
            Return convertToSequenceField
        End Function

        Public Sub setConvertToSequence(ByVal convertToSequence As ConvertToSequenceT)
            Me.convertToSequenceField = convertToSequence
        End Sub

        Public Function getConvertFromSequence() As ConvertFromSequenceT
            Return convertFromSequenceField
        End Function

        Public Sub setConvertFromSequence(ByVal convertFromSequence As ConvertFromSequenceT)
            Me.convertFromSequenceField = convertFromSequence
        End Sub

        Public Function getSequenceSplit() As SequenceSplitT
            Return sequenceSplitField
        End Function

        Public Sub setSequenceSplit(ByVal sequenceSplit As SequenceSplitT)
            Me.sequenceSplitField = sequenceSplit
        End Sub

        Public Function getReducer() As IAssociativeReducerT
            Return reducerField
        End Function

        Public Sub setReducer(ByVal reducer As IAssociativeReducerT)
            Me.reducerField = reducer
        End Sub

        Public Function getCalculateSortedRank() As CalculateSortedRankT
            Return calculateSortedRankField
        End Function

        Public Sub setCalculateSortedRank(ByVal calculateSortedRank As CalculateSortedRankT)
            Me.calculateSortedRankField = calculateSortedRank
        End Sub

        Public Overrides Function ToString() As String
            Dim str As String
            If transformField IsNot Nothing Then
                str = transformField.ToString()
            ElseIf filterField IsNot Nothing Then
                str = filterField.ToString()
            ElseIf convertToSequenceField IsNot Nothing Then
                str = convertToSequenceField.ToString()
            ElseIf convertFromSequenceField IsNot Nothing Then
                str = convertFromSequenceField.ToString()
            ElseIf sequenceSplitField IsNot Nothing Then
                str = sequenceSplitField.ToString()
            ElseIf reducerField IsNot Nothing Then
                str = reducerField.ToString()
            ElseIf calculateSortedRankField IsNot Nothing Then
                str = calculateSortedRankField.ToString()
            Else
                Throw New InvalidOperationException("Invalid DataAction: does not contain any operation to perform (all fields are null)")
            End If
            Return "DataAction(" & str & ")"
        End Function

        ''' <summary>The input schema of the operation of this step.</summary>
        ''' <exception cref="InvalidOperationException">if no operation is set</exception>
        Public Overridable ReadOnly Property Schema As SchemaT
            Get
                If transformField IsNot Nothing Then
                    Return transformField.InputSchema
                ElseIf filterField IsNot Nothing Then
                    Return filterField.InputSchema
                ElseIf convertToSequenceField IsNot Nothing Then
                    Return convertToSequenceField.getInputSchema()
                ElseIf convertFromSequenceField IsNot Nothing Then
                    Return convertFromSequenceField.getInputSchema()
                ElseIf sequenceSplitField IsNot Nothing Then
                    Return sequenceSplitField.InputSchema
                ElseIf reducerField IsNot Nothing Then
                    Return reducerField.InputSchema
                ElseIf calculateSortedRankField IsNot Nothing Then
                    Return calculateSortedRankField.InputSchema
                Else
                    Throw New InvalidOperationException("Invalid DataAction: does not contain any operation to perform (all fields are null)")
                End If
            End Get
        End Property

        ''' <summary>Java <c>getSchema()</c>.</summary>
        Public Function getSchema() As SchemaT
            Return Schema
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other As DataAction = TryCast(obj, DataAction)
            If other Is Nothing OrElse CObj(other).GetType() IsNot CObj(Me).GetType() Then Return False
            Return Object.Equals(transformField, other.transformField) AndAlso
                   Object.Equals(filterField, other.filterField) AndAlso
                   Object.Equals(convertToSequenceField, other.convertToSequenceField) AndAlso
                   Object.Equals(convertFromSequenceField, other.convertFromSequenceField) AndAlso
                   Object.Equals(sequenceSplitField, other.sequenceSplitField) AndAlso
                   Object.Equals(reducerField, other.reducerField) AndAlso
                   Object.Equals(calculateSortedRankField, other.calculateSortedRankField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(transformField, filterField, convertToSequenceField, convertFromSequenceField, sequenceSplitField, reducerField, calculateSortedRankField)
        End Function

    End Class

End Namespace
