Imports System
Imports System.Text
Imports Getter = lombok.Getter
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JCudaBuffer = org.nd4j.linalg.jcublas.buffer.JCudaBuffer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports Slf4j = lombok.extern.slf4j.Slf4j

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

Namespace org.nd4j.linalg.jcublas

	''' <summary>
	''' Wraps the allocation
	''' and freeing of resources on a cuda device
	''' @author bam4d
	''' 
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CublasPointer implements AutoCloseable
	Public Class CublasPointer
		Implements AutoCloseable

		''' <summary>
		''' The underlying cuda buffer that contains the host and device memory
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field buffer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private buffer_Conflict As JCudaBuffer
'JAVA TO VB CONVERTER NOTE: The field devicePointer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private devicePointer_Conflict As Pointer
'JAVA TO VB CONVERTER NOTE: The field hostPointer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hostPointer_Conflict As Pointer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean closed = false;
		Private closed As Boolean = False
		Private arr As INDArray
		Private cudaContext As CudaContext
'JAVA TO VB CONVERTER NOTE: The field resultPointer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private resultPointer_Conflict As Boolean = False


		''' <summary>
		''' frees the underlying
		''' device memory allocated for this pointer
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws Exception
		Public Overrides Sub close()
			If Not ResultPointer Then
				destroy()
			End If
		End Sub


		''' <summary>
		''' The actual destroy method
		''' </summary>
		Public Overridable Sub destroy()

		End Sub


		''' 
		''' <summary>
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Buffer As JCudaBuffer
			Get
				Return buffer_Conflict
			End Get
		End Property

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property DevicePointer As Pointer
			Get
				Return devicePointer_Conflict
			End Get
		End Property

		Public Overridable Property HostPointer As Pointer
			Get
				Return hostPointer_Conflict
			End Get
			Set(ByVal hostPointer As Pointer)
				Me.hostPointer_Conflict = hostPointer
			End Set
		End Property


		''' <summary>
		''' Creates a CublasPointer
		''' for a given JCudaBuffer </summary>
		''' <param name="buffer"> </param>
		Public Sub New(ByVal buffer As JCudaBuffer, ByVal context As CudaContext)
			Me.buffer_Conflict = buffer
			'        this.devicePointer = AtomicAllocator.getInstance().getPointer(new Pointer(buffer.originalDataBuffer() == null ? buffer : buffer.originalDataBuffer()), AllocationUtils.buildAllocationShape(buffer), true);
			Me.cudaContext = context
	'        
	'        context.initOldStream();
	'        
	'        DevicePointerInfo info = buffer.getPointersToContexts().get(Thread.currentThread().getName(), Triple.of(0, buffer.length(), 1));
	'        hostPointer = info.getPointers().getHostPointer();
	'        ContextHolder.getInstance().getMemoryStrategy().setData(devicePointer,0,1,buffer.length(),info.getPointers().getHostPointer());
	'        buffer.setCopied(Thread.currentThread().getName());
	'        
		End Sub

		''' <summary>
		''' Creates a CublasPointer for a given INDArray.
		''' 
		''' This wrapper makes sure that the INDArray offset, stride
		''' and memory pointers are accurate to the data being copied to and from the device.
		''' 
		''' If the copyToHost function is used in this class,
		''' the host buffer offset and data length is taken care of automatically </summary>
		''' <param name="array"> </param>
		Public Sub New(ByVal array As INDArray, ByVal context As CudaContext)
			'we have to reset the pointer to be zero offset due to the fact that
			'vector based striding won't work with an array that looks like this

			Me.cudaContext = context
			Me.devicePointer_Conflict = AtomicAllocator.Instance.getPointer(array, context)

		End Sub


		''' <summary>
		''' Whether this is a result pointer or not
		''' A result pointer means that this
		''' pointer should not automatically be freed
		''' but instead wait for results to accumulate
		''' so they can be returned from
		''' the gpu first
		''' @return
		''' </summary>
		Public Overridable Property ResultPointer As Boolean
			Get
				Return resultPointer_Conflict
			End Get
			Set(ByVal resultPointer As Boolean)
				Me.resultPointer_Conflict = resultPointer
			End Set
		End Property


		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("NativePointer: [" & devicePointer_Conflict.address() & "]")
			Return sb.ToString()
		End Function


		Public Shared Sub free(ParamArray ByVal pointers() As CublasPointer)
			For Each pointer As CublasPointer In pointers
				Try
					pointer.close()
				Catch e As Exception
					log.error("",e)
				End Try
			Next pointer
		End Sub


	End Class

End Namespace