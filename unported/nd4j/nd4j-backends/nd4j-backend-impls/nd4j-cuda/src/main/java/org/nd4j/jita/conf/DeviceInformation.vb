Imports Data = lombok.Data

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

Namespace org.nd4j.jita.conf


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class DeviceInformation
	Public Class DeviceInformation
		Private deviceId As Integer

		Private ccMajor As Integer = 0
		Private ccMinor As Integer = 0

		''' <summary>
		''' Total amount of memory available on current specific device
		''' </summary>
		Private totalMemory As Long = 0

		''' <summary>
		''' Available RAM
		''' </summary>
		Private availableMemory As Long = 0

		''' <summary>
		''' This is amount of RAM allocated within current JVM process
		''' </summary>
		Private allocatedMemory As New AtomicLong(0)

	'    
	'        Key features we care about: hostMapped, overlapped exec, number of cores/sm
	'     
		Private canMapHostMemory As Boolean = False

		Private overlappedKernels As Boolean = False

		Private concurrentKernels As Boolean = False

		Private sharedMemPerBlock As Long = 0

		Private sharedMemPerMP As Long = 0

		Private warpSize As Integer = 0
	End Class

End Namespace