Imports System
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
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

Namespace org.deeplearning4j.nn.layers.mkldnn


	Public Class BaseMKLDNNHelper

		Private Shared BACKEND_OK As AtomicBoolean = Nothing
		Private Shared FAILED_CHECK As AtomicBoolean = Nothing

		Public Shared Function mklDnnEnabled() As Boolean
			If BACKEND_OK Is Nothing Then
				Dim backend As String = Nd4j.Executioner.EnvironmentInformation.getProperty("backend")
				BACKEND_OK = New AtomicBoolean("CPU".Equals(backend, StringComparison.OrdinalIgnoreCase))
			End If

			If Not BACKEND_OK.get() OrElse (FAILED_CHECK IsNot Nothing AndAlso FAILED_CHECK.get()) Then
				'Wrong backend or already failed trying to check
				Return False
			End If

			If Not Nd4j.Environment.helpersAllowed() Then
				'C++ helpers not allowed
				Return False
			End If

			Try
				Dim clazz As Type = DL4JClassLoading.loadClassByName("org.nd4j.nativeblas.Nd4jCpu$Environment")
				Dim getInstance As System.Reflection.MethodInfo = clazz.GetMethod("getInstance")
				Dim instance As Object = getInstance.invoke(Nothing)
				Dim isUseMKLDNNMethod As System.Reflection.MethodInfo = clazz.GetMethod("isUseMKLDNN")
				Return CBool(isUseMKLDNNMethod.invoke(instance))
			Catch t As Exception
				FAILED_CHECK = New AtomicBoolean(True)
				Return False
			End Try
		End Function

	End Class

End Namespace