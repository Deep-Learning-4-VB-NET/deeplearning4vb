Imports System
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports ND4VBSystemProperties = DeepLearning4VB.org.nd4vb.common.config.ND4VBSystemProperties
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.api.buffer.util

    ''' <summary>
    ''' Manipulates the data type from the configuration (Java <c>DataTypeUtil</c>).
    ''' </summary>
    ''' <remarks>
    ''' Java read and wrote the <c>dtype</c> key of the <c>Nd4jContext</c> configuration; ND4VB uses the backend properties
    ''' (<c>Nd4vb.Backend.Properties</c>, see <c>Nd4vbBackend.Properties</c>). The value read is cached as in Java. The Java
    ''' setters <c>setDTypeForContext</c> are methods (VB properties cannot be overloaded by type). A missing <c>dtype</c>
    ''' property gives FLOAT (Java threw a <c>NullPointerException</c> in its <c>switch</c>).
    ''' </remarks>
    Public Class DataTypeUtil

        Private Shared dtype As DataType?
        Private Shared ReadOnly gate As New Object()

        ''' <summary>Returns the length for the given data type: 8 for DOUBLE and LONG, 4 for FLOAT and INT, 2 for HALF.</summary>
        ''' <exception cref="ArgumentException">Another data type was given.</exception>
        Public Shared Function lengthForDtype(ByVal type As DataType) As Integer
            Select Case type
                Case DataType.DOUBLE
                    Return 8
                Case DataType.FLOAT
                    Return 4
                Case DataType.INT
                    Return 4
                Case DataType.HALF
                    Return 2
                Case DataType.LONG
                    Return 8
                Case Else
                    Throw New ArgumentException("Illegal opType for length")
            End Select
        End Function

        ''' <summary>Get the allocation mode from the context: "double", "float", "int" or "half" (anything else gives FLOAT).</summary>
        Public Shared Function getDtypeFromContext(ByVal dType As String) As DataType
            Select Case dType
                Case "double"
                    Return DataType.DOUBLE
                Case "float"
                    Return DataType.FLOAT
                Case "int"
                    Return DataType.INT
                Case "half"
                    Return DataType.HALF
                Case Else
                    Return DataType.FLOAT
            End Select
        End Function

        ''' <summary>Gets the name of the data type ("double", "float", "int" or "half"; "float" for the others).</summary>
        Public Shared Function getDTypeForName(ByVal allocationMode As DataType) As String
            Select Case allocationMode
                Case DataType.DOUBLE
                    Return "double"
                Case DataType.FLOAT
                    Return "float"
                Case DataType.INT
                    Return "int"
                Case DataType.HALF
                    Return "half"
                Case Else
                    Return "float"
            End Select
        End Function

        ''' <summary>The data type in the backend properties (read once, then cached).</summary>
        Public Shared ReadOnly Property DtypeFromContext As DataType
            Get
                SyncLock gate
                    If Not dtype.HasValue Then
                        dtype = getDtypeFromContext(Nd4vbT.Backend.Properties.getProperty(ND4VBSystemProperties.DTYPE))
                    End If
                    Return dtype.Value
                End SyncLock
            End Get
        End Property

        ''' <summary>Java <c>getDtypeFromContext()</c>.</summary>
        Public Shared Function getDtypeFromContext() As DataType
            Return DtypeFromContext
        End Function

        ''' <summary>Sets the data type in the cache and in the backend properties.</summary>
        Public Shared Sub setDTypeForContext(ByVal allocationModeForContext As DataType)
            SyncLock gate
                dtype = allocationModeForContext
                setDTypeForContext(getDTypeForName(allocationModeForContext))
            End SyncLock
        End Sub

        ''' <summary>Sets the data type name ("double", "float", "int" or "half") in the backend properties.</summary>
        ''' <exception cref="ArgumentException">Another value was given.</exception>
        Public Shared Sub setDTypeForContext(ByVal allocationModeForContext As String)
            If Not "double".Equals(allocationModeForContext) AndAlso Not "float".Equals(allocationModeForContext) AndAlso Not "int".Equals(allocationModeForContext) AndAlso Not "half".Equals(allocationModeForContext) Then
                Throw New ArgumentException("Allocation mode must be one of: double,float, or int")
            End If
            Nd4vbT.Backend.Properties.put(ND4VBSystemProperties.DTYPE, allocationModeForContext)
        End Sub

    End Class

End Namespace
