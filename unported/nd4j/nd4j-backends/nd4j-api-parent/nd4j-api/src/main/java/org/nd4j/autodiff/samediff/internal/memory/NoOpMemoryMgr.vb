Imports NonNull = lombok.NonNull
Imports SessionMemMgr = org.nd4j.autodiff.samediff.internal.SessionMemMgr
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.autodiff.samediff.internal.memory

	Public Class NoOpMemoryMgr
		Inherits AbstractMemoryMgr
		Implements SessionMemMgr

		Public Overrides Function allocate(ByVal detached As Boolean, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray Implements SessionMemMgr.allocate
			Return Nd4j.createUninitialized(dataType, shape)
		End Function

		Public Overrides Function allocate(ByVal detached As Boolean, ByVal descriptor As LongShapeDescriptor) As INDArray Implements SessionMemMgr.allocate
			Return Nd4j.create(descriptor, False)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void release(@NonNull INDArray array)
		Public Overridable Overloads Sub release(ByVal array As INDArray)
			'No-op, rely on GC to clear arrays
		End Sub

		Public Overrides Sub Dispose() Implements SessionMemMgr.close
			'No-op
		End Sub

	End Class
End Namespace