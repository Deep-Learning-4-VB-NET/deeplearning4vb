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

Namespace org.nd4j.linalg.api.blas

	Public Interface Blas

		Public Enum Vendor

			UNKNOWN
			CUBLAS
			OPENBLAS
			MKL
		End Enum

		Property MaxThreads As Integer


		''' <summary>
		''' Returns the BLAS library vendor id
		''' 
		''' 0 - UNKNOWN, 1 - CUBLAS, 2 - OPENBLAS, 3 - MKL
		''' </summary>
		''' <returns> the BLAS library vendor id </returns>
		ReadOnly Property BlasVendorId As Integer

		''' <summary>
		''' Returns the BLAS library vendor
		''' </summary>
		''' <returns> the BLAS library vendor </returns>
		ReadOnly Property BlasVendor As Vendor
	End Interface

End Namespace