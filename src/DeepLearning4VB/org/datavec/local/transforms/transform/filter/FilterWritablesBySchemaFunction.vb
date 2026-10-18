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

Imports DeepLearning4VB.org.nd4vb.common.function
Imports ColumnMetaDataT = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports NullWritableT = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.transform.filter

    ''' <summary>
    ''' Keeps the values of a column that are valid for its metadata (or, with <c>keepValid</c> false, the invalid
    ''' ones), optionally excluding missing values (<c>NullWritable</c> or empty text) whatever their validity.
    ''' </summary>
    Public Class FilterWritablesBySchemaFunction
        Implements [Function](Of WritableT, Boolean)

        Private ReadOnly meta As ColumnMetaDataT
        Private ReadOnly keepValid As Boolean 'If true: keep valid. If false: keep invalid
        Private ReadOnly excludeMissing As Boolean 'If true: remove/exclude any

        Public Sub New(ByVal meta As ColumnMetaDataT, ByVal keepValid As Boolean)
            Me.New(meta, keepValid, False)
        End Sub

        ''' <param name="meta">              Column meta data </param>
        ''' <param name="keepValid">         If true: keep only the valid writables. If false: keep only the invalid writables </param>
        ''' <param name="excludeMissing">    If true: don't return any missing values, regardless of keepValid setting (i.e., exclude any NullWritable or empty string values) </param>
        Public Sub New(ByVal meta As ColumnMetaDataT, ByVal keepValid As Boolean, ByVal excludeMissing As Boolean)
            Me.meta = meta
            Me.keepValid = keepValid
            Me.excludeMissing = excludeMissing
        End Sub

        Public Overridable Function apply(ByVal v1 As WritableT) As Boolean Implements [Function](Of WritableT, Boolean).apply
            Dim valid As Boolean = meta.isValid(v1)
            If excludeMissing AndAlso (TypeOf v1 Is NullWritableT OrElse (TypeOf v1 Is TextT AndAlso String.IsNullOrEmpty(v1.ToString()))) Then
                Return False 'Remove
            End If

            If keepValid Then
                Return valid 'return true to keep
            Else
                Return Not valid
            End If
        End Function

    End Class

End Namespace
