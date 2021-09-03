Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor

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

Namespace org.nd4j.autodiff.samediff.internal


	Public Interface SessionMemMgr
		Inherits System.IDisposable

		''' <summary>
		''' Allocate an array with the specified datatype and shape.<br>
		''' NOTE: This array should be assumed to be uninitialized - i.e., contains random values.
		''' </summary>
		''' <param name="detached"> If true: the array is safe to return outside of the SameDiff session run (for example, the array
		'''                 is one that may be returned to the user) </param>
		''' <param name="dataType"> Datatype of the returned array </param>
		''' <param name="shape">    Array shape </param>
		''' <returns> The newly allocated (uninitialized) array </returns>
		Function allocate(ByVal detached As Boolean, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray

		''' <summary>
		''' As per <seealso cref="allocate(Boolean, DataType, Long...)"/> but from a LongShapeDescriptor instead
		''' </summary>
		Function allocate(ByVal detached As Boolean, ByVal descriptor As LongShapeDescriptor) As INDArray

		''' <summary>
		''' Allocate an uninitialized array with the same datatype and shape as the specified array
		''' </summary>
		Function ulike(ByVal arr As INDArray) As INDArray

		''' <summary>
		''' Duplicate the specified array, to an array that is managed/allocated by the session memory manager
		''' </summary>
		Function dup(ByVal arr As INDArray) As INDArray

		''' <summary>
		''' Release the array. All arrays allocated via one of the allocate methods should be returned here once they are no
		''' longer used, and all references to them should be cleared.
		''' After calling release, anything could occur to the array - deallocated, workspace closed, reused, etc.
		''' </summary>
		''' <param name="array"> The array that can be released </param>
		Sub release(ByVal array As INDArray)

		''' <summary>
		''' Close the session memory manager and clean up any memory / resources, if any
		''' </summary>
		Sub close()

	End Interface

End Namespace