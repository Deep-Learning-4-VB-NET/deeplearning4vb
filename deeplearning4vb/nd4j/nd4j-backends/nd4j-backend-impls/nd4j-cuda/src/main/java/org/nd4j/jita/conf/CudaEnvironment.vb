Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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


	''' 
	''' <summary>
	''' The cuda environment contains information
	''' for a given <seealso cref="Configuration"/>
	''' singleton.
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class CudaEnvironment
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New CudaEnvironment()
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private static volatile Configuration configuration;
'JAVA TO VB CONVERTER NOTE: The field configuration was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared configuration_Conflict As Configuration
		Private Shared arch As IDictionary(Of Integer, Integer) = New ConcurrentDictionary(Of Integer, Integer)()

		Private Sub New()
			configuration_Conflict = New Configuration()

		End Sub

		Public Shared ReadOnly Property Instance As CudaEnvironment
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		''' <summary>
		''' Get the <seealso cref="Configuration"/>
		''' for the environment
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Configuration As Configuration
			Get
				Return configuration_Conflict
			End Get
		End Property

		''' <summary>
		''' Get the current device architecture </summary>
		''' <returns> the major/minor version of
		''' the current device </returns>
		Public Overridable ReadOnly Property CurrentDeviceArchitecture As Integer
			Get
				Dim deviceId As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()
				If Not arch.ContainsKey(deviceId) Then
					Dim major As Integer = NativeOpsHolder.Instance.getDeviceNativeOps().getDeviceMajor(deviceId)
					Dim minor As Integer = NativeOpsHolder.Instance.getDeviceNativeOps().getDeviceMinor(deviceId)
					Dim cc As Integer? = Integer.Parse("" & major + minor)
					arch(deviceId) = cc
					Return cc
				End If
    
				Return arch(deviceId)
			End Get
		End Property

		Public Overridable Sub notifyConfigurationApplied()
			configuration_Conflict.updateDevice()
		End Sub
	End Class

End Namespace