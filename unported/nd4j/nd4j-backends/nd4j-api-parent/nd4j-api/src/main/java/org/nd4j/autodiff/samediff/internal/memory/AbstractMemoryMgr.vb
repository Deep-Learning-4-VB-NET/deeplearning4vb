Imports NonNull = lombok.NonNull
Imports SessionMemMgr = org.nd4j.autodiff.samediff.internal.SessionMemMgr
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

	Public MustInherit Class AbstractMemoryMgr
		Implements SessionMemMgr

		Public MustOverride Sub close() Implements SessionMemMgr.close
		Public MustOverride Sub release(ByVal array As INDArray) Implements SessionMemMgr.release
		Public MustOverride Function allocate(ByVal detached As Boolean, ByVal descriptor As org.nd4j.linalg.api.shape.LongShapeDescriptor) As INDArray
		Public MustOverride Function allocate(ByVal detached As Boolean, ByVal dataType As org.nd4j.linalg.api.buffer.DataType, ByVal shape() As Long) As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray ulike(@NonNull INDArray arr)
		Public Overridable Function ulike(ByVal arr As INDArray) As INDArray
			Return allocate(False, arr.dataType(), arr.shape())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray dup(@NonNull INDArray arr)
		Public Overridable Function dup(ByVal arr As INDArray) As INDArray
			Dim [out] As INDArray = ulike(arr)
			[out].assign(arr)
			Return [out]
		End Function
	End Class

End Namespace