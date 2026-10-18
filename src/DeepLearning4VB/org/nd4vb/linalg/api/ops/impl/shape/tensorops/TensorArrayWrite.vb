Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.shape.tensorops

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.shape.tensorops.TensorArrayWrite</c>.</summary>
    Public Class TensorArrayWrite
        Inherits BaseTensorOp

#If SAMEDIFF Then
	   Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
		  MyBase.New(name, sameDiff, args)
	   End Sub
	   Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
		  MyBase.New(Nothing, sameDiff, args)
	   End Sub
#End If

       Public Sub New()
       End Sub
       Public Overrides Function tensorflowNames() As String()
          Return New String(){"TensorArrayWrite", "TensorArrayWriteV2", "TensorArrayWriteV3"}
       End Function

       Public Overrides Function opName() As String
          Return "tensorarraywritev3"
       End Function

       Public Overrides Function opType() As Op.Type
          Return Op.Type.CUSTOM
       End Function

       Public Overrides Function calculateOutputDataTypes(ByVal inputDataType As IList(Of DataType)) As IList(Of DataType)
          'Dummy float variable
          Return New List(Of DataType) From {DataType.FLOAT}
       End Function

    End Class

End Namespace
