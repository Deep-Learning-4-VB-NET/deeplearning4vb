Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Loader = org.bytedeco.javacpp.Loader
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports Blas = org.nd4j.linalg.api.blas.Blas

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

Namespace org.nd4j.nativeblas



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class Nd4jBlas implements org.nd4j.linalg.api.blas.Blas
	Public MustInherit Class Nd4jBlas
		Implements Blas

		Public MustOverride ReadOnly Property BlasVendorId As Integer Implements Blas.getBlasVendorId
		Public MustOverride Property MaxThreads As Integer Implements Blas.getMaxThreads


		Public Sub New()
			Dim numThreads As Integer
			Dim skipper As String = Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_SKIP_BLAS_THREADS)
			If skipper Is Nothing OrElse skipper.Length = 0 Then
				Dim numThreadsString As String = Environment.GetEnvironmentVariable(ND4JEnvironmentVars.OMP_NUM_THREADS)
				If numThreadsString IsNot Nothing AndAlso numThreadsString.Length > 0 Then
					numThreads = Integer.Parse(numThreadsString)
					MaxThreads = numThreads
				Else
					Dim cores As Integer = Loader.totalCores()
					Dim chips As Integer = Loader.totalChips()
					If cores > 0 AndAlso chips > 0 Then
						numThreads = Math.Max(1, cores \ chips)
					Else
						numThreads = NativeOpsHolder.getCores(Runtime.getRuntime().availableProcessors())
					End If
					MaxThreads = numThreads
				End If

				Dim logInit As String = System.getProperty(ND4JSystemProperties.LOG_INITIALIZATION)
				If logOpenMPBlasThreads() AndAlso (logInit Is Nothing OrElse logInit.Length = 0 OrElse Boolean.Parse(logInit)) Then
					log.info("Number of threads used for OpenMP BLAS: {}", MaxThreads)
				End If
			End If
		End Sub

		''' <summary>
		''' Returns the BLAS library vendor
		''' </summary>
		''' <returns> the BLAS library vendor </returns>
		Public Overridable ReadOnly Property BlasVendor As Vendor Implements Blas.getBlasVendor
			Get
				Dim vendor As Integer = BlasVendorId
				Dim isUnknowVendor As Boolean = ((vendor > System.Enum.GetValues(GetType(Vendor)).length - 1) OrElse (vendor <= 0))
				If isUnknowVendor Then
					Return Vendor.UNKNOWN
				End If
				Return System.Enum.GetValues(GetType(Vendor))(vendor)
			End Get
		End Property

		Public Overridable Function logOpenMPBlasThreads() As Boolean
			Return True
		End Function
	End Class

End Namespace