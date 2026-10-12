Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer

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

Namespace org.nd4j.linalg.jcublas.util

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class FFTUtils
		''' <summary>
		''' Get the plan for the given buffer (C2C for float Z2Z for double) </summary>
		''' <param name="buff"> the buffer to get the plan for </param>
		''' <returns> the plan for the given buffer </returns>
		Public Shared Function getPlanFor(ByVal buff As DataBuffer) As Integer
	'           if(buff.dataType() == DataType.FLOAT)
	'            return cufftType.CUFFT_C2C;
	'        else
	'            return cufftType.CUFFT_Z2Z;
	'            
			Throw New System.NotSupportedException()
		End Function


	End Class

End Namespace