Imports Level2 = org.nd4j.linalg.api.blas.Level2
Imports GemvParameters = org.nd4j.linalg.api.blas.params.GemvParameters
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DefaultOpExecutioner = org.nd4j.linalg.api.ops.executioner.DefaultOpExecutioner
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports OpExecutionerUtil = org.nd4j.linalg.api.ops.executioner.OpExecutionerUtil
Imports Mmul = org.nd4j.linalg.api.ops.impl.reduce.Mmul
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports OpProfiler = org.nd4j.linalg.profiler.OpProfiler

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

Namespace org.nd4j.linalg.api.blas.impl

	Public MustInherit Class BaseLevel2
		Inherits BaseLevel
		Implements Level2

		''' <summary>
		''' gemv computes a matrix-vector product using a general matrix and performs one of the following matrix-vector operations:
		''' y := alpha*a*x + beta*y  for trans = 'N'or'n';
		''' y := alpha*a'*x + beta*y  for trans = 'T'or't';
		''' y := alpha*conjg(a')*x + beta*y  for trans = 'C'or'c'.
		''' Here a is an m-by-n band matrix, x and y are vectors, alpha and beta are scalars.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="transA"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		''' <param name="beta"> </param>
		''' <param name="Y"> </param>
		Public Overridable Sub gemv(ByVal order As Char, ByVal transA As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal X As INDArray, ByVal beta As Double, ByVal Y As INDArray) Implements Level2.gemv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X, Y)
			End If

			Dim parameters As New GemvParameters(A, X, Y)

			Nd4j.exec(New Mmul(A, X, Y, alpha, beta, MMulTranspose.builder().transposeA(False).build()))

	'        
	'        if (A.data().dataType() == DataType.DOUBLE) {
	'            DefaultOpExecutioner.validateDataType(DataType.DOUBLE, parameters.getA(), parameters.getX(),
	'                            parameters.getY());
	'            dgemv(order, parameters.getAOrdering(), parameters.getM(), parameters.getN(), alpha, parameters.getA(),
	'                            parameters.getLda(), parameters.getX(), parameters.getIncx(), beta, parameters.getY(),
	'                            parameters.getIncy());
	'        } else if (A.data().dataType() == DataType.FLOAT){
	'            DefaultOpExecutioner.validateDataType(DataType.FLOAT, parameters.getA(), parameters.getX(),
	'                            parameters.getY());
	'            sgemv(order, parameters.getAOrdering(), parameters.getM(), parameters.getN(), (float) alpha,
	'                            parameters.getA(), parameters.getLda(), parameters.getX(), parameters.getIncx(),
	'                            (float) beta, parameters.getY(), parameters.getIncy());
	'        } else if (A.data().dataType() == DataType.HALF) {
	'            DefaultOpExecutioner.validateDataType(DataType.HALF, parameters.getA(), parameters.getX(),
	'                    parameters.getY());
	'
	'            // TODO: provide optimized GEMV kernel eventually
	'            val fA = parameters.getA().castTo(DataType.FLOAT);
	'            val fX = parameters.getX().castTo(DataType.FLOAT);
	'            val fY = parameters.getY().castTo(DataType.FLOAT);
	'
	'            sgemv(order, parameters.getAOrdering(), parameters.getM(), parameters.getN(), (float) alpha,
	'                    fA, parameters.getLda(), fX, parameters.getIncx(),
	'                    (float) beta, fY, parameters.getIncy());
	'
	'            Y.assign(fY);
	'        } else {
	'            throw new ND4JIllegalStateException("Unsupported data type " + A.dataType());
	'        }
	'        
			OpExecutionerUtil.checkForAny(Y)
		End Sub

		''' <summary>
		''' gbmv computes a matrix-vector product using a general band matrix and performs one of the following matrix-vector operations:
		''' y := alpha*a*x + beta*y  for trans = 'N'or'n';
		''' y := alpha*a'*x + beta*y  for trans = 'T'or't';
		''' y := alpha*conjg(a')*x + beta*y  for trans = 'C'or'c'.
		''' Here a is an m-by-n band matrix with ku superdiagonals and kl subdiagonals, x and y are vectors, alpha and beta are scalars.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="TransA"> </param>
		''' <param name="KL"> </param>
		''' <param name="KU"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		''' <param name="beta"> </param>
		''' <param name="Y"> </param>
		Public Overridable Sub gbmv(ByVal order As Char, ByVal TransA As Char, ByVal KL As Integer, ByVal KU As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal X As INDArray, ByVal beta As Double, ByVal Y As INDArray) Implements Level2.gbmv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X, Y)
			End If

			If A.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X, Y)
				If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				dgbmv(order, TransA, CInt(A.rows()), CInt(A.columns()), KL, KU, alpha, A, CInt(A.size(0)), X, X.stride(-1), beta, Y, Y.stride(-1))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X, Y)
				sgbmv(order, TransA, CInt(A.rows()), CInt(A.columns()), KL, KU, CSng(alpha), A, CInt(A.size(0)), X, X.stride(-1), CSng(beta), Y, Y.stride(-1))
			End If

			OpExecutionerUtil.checkForAny(Y)
		End Sub

		''' <summary>
		''' performs a rank-1 update of a general m-by-n matrix a:
		''' a := alpha*x*y' + a.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="Y"> </param>
		''' <param name="A"> </param>
		Public Overridable Sub ger(ByVal order As Char, ByVal alpha As Double, ByVal X As INDArray, ByVal Y As INDArray, ByVal A As INDArray) Implements Level2.ger
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X, Y)
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X, Y)
				If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				dger(order, CInt(A.rows()), CInt(A.columns()), alpha, X, X.stride(-1), Y, Y.stride(-1), A, CInt(A.size(0)))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X, Y)
				sger(order, CInt(A.rows()), CInt(A.columns()), CSng(alpha), X, X.stride(-1), Y, Y.stride(-1), A, CInt(A.size(0)))
			End If

			OpExecutionerUtil.checkForAny(A)
		End Sub

		''' <summary>
		''' sbmv computes a matrix-vector product using a symmetric band matrix:
		''' y := alpha*a*x + beta*y.
		''' Here a is an n-by-n symmetric band matrix with k superdiagonals, x and y are n-element vectors, alpha and beta are scalars.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		''' <param name="beta"> </param>
		''' <param name="Y"> </param>
		Public Overridable Sub sbmv(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal X As INDArray, ByVal beta As Double, ByVal Y As INDArray) Implements Level2.sbmv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X, Y)
			End If

			If X.length() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X, Y)
				dsbmv(order, Uplo, CInt(X.length()), CInt(A.columns()), alpha, A, CInt(A.size(0)), X, X.stride(-1), beta, Y, Y.stride(-1))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X, Y)
				ssbmv(order, Uplo, CInt(X.length()), CInt(A.columns()), CSng(alpha), A, CInt(A.size(0)), X, X.stride(-1), CSng(beta), Y, Y.stride(-1))
			End If

			OpExecutionerUtil.checkForAny(Y)
		End Sub

		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="Ap"> </param>
		''' <param name="X"> </param>
		''' <param name="beta"> </param>
		''' <param name="Y"> </param>
		Public Overridable Sub spmv(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal Ap As INDArray, ByVal X As INDArray, ByVal beta As Double, ByVal Y As INDArray) Implements Level2.spmv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, Ap, X, Y)
			End If

			If X.length() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If Ap.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, X, Y)
				dspmv(order, Uplo, CInt(X.length()), alpha, Ap, X, Ap.stride(-1), beta, Y, Y.stride(-1))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, X, Y)
				sspmv(order, Uplo, CInt(X.length()), CSng(alpha), Ap, X, Ap.stride(-1), CSng(beta), Y, Y.stride(-1))
			End If

			OpExecutionerUtil.checkForAny(Y)
		End Sub

		''' <summary>
		''' spr performs a rank-1 update of an n-by-n packed symmetric matrix a:
		''' a := alpha*x*x' + a.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="Ap"> </param>
		Public Overridable Sub spr(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal X As INDArray, ByVal Ap As INDArray) Implements Level2.spr
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, Ap, X)
			End If


			If X.length() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, X)
				dspr(order, Uplo, CInt(X.length()), alpha, X, X.stride(-1), Ap)
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, X)
				sspr(order, Uplo, CInt(X.length()), CSng(alpha), X, X.stride(-1), Ap)
			End If

			OpExecutionerUtil.checkForAny(Ap)
		End Sub

		''' <summary>
		''' ?spr2 performs a rank-2 update of an n-by-n packed symmetric matrix a:
		''' a := alpha*x*y' + alpha*y*x' + a.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="Y"> </param>
		''' <param name="A"> </param>
		Public Overridable Sub spr2(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal X As INDArray, ByVal Y As INDArray, ByVal A As INDArray) Implements Level2.spr2
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X, Y)
			End If

			If X.length() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X, Y)
				dspr2(order, Uplo, CInt(X.length()), alpha, X, X.stride(-1), Y, Y.stride(-1), A)
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X, Y)
				sspr2(order, Uplo, CInt(X.length()), CSng(alpha), X, X.stride(-1), Y, Y.stride(-1), A)
			End If

			OpExecutionerUtil.checkForAny(A)
		End Sub

		''' <summary>
		''' symv computes a matrix-vector product for a symmetric matrix:
		''' y := alpha*a*x + beta*y.
		''' Here a is an n-by-n symmetric matrix; x and y are n-element vectors, alpha and beta are scalars.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		''' <param name="beta"> </param>
		''' <param name="Y"> </param>
		Public Overridable Sub symv(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal X As INDArray, ByVal beta As Double, ByVal Y As INDArray) Implements Level2.symv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X, Y)
			End If

			If X.length() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X, Y)
				dsymv(order, Uplo, CInt(X.length()), alpha, A, CInt(A.size(0)), X, X.stride(-1), beta, Y, Y.stride(-1))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X, Y)
				ssymv(order, Uplo, CInt(X.length()), CSng(alpha), A, CInt(A.size(0)), X, X.stride(-1), CSng(beta), Y, Y.stride(-1))
			End If

			OpExecutionerUtil.checkForAny(Y)
		End Sub

		''' <summary>
		''' syr performs a rank-1 update of an n-by-n symmetric matrix a:
		''' a := alpha*x*x' + a.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="N"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="A"> </param>
		Public Overridable Sub syr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal A As INDArray) Implements Level2.syr
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X)
			End If

			If X.length() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X)
				dsyr(order, Uplo, CInt(X.length()), alpha, X, X.stride(-1), A, CInt(A.size(0)))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X)
				ssyr(order, Uplo, CInt(X.length()), CSng(alpha), X, X.stride(-1), A, CInt(A.size(0)))
			End If

			OpExecutionerUtil.checkForAny(A)
		End Sub

		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="Y"> </param>
		''' <param name="A"> </param>
		Public Overridable Sub syr2(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal X As INDArray, ByVal Y As INDArray, ByVal A As INDArray) Implements Level2.syr2
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X, Y)
			End If

			If X.length() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X, Y)
				dsyr2(order, Uplo, CInt(X.length()), alpha, X, X.stride(-1), Y, Y.stride(-1), A, CInt(A.size(0)))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X, Y)
				ssyr2(order, Uplo, CInt(X.length()), CSng(alpha), X, X.stride(-1), Y, Y.stride(-1), A, CInt(A.size(0)))
			End If

			OpExecutionerUtil.checkForAny(A)
		End Sub

		''' <summary>
		''' syr2 performs a rank-2 update of an n-by-n symmetric matrix a:
		''' a := alpha*x*y' + alpha*y*x' + a.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		Public Overridable Sub tbmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal A As INDArray, ByVal X As INDArray) Implements Level2.tbmv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X)
			End If

			If X.length() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X)
				dtbmv(order, Uplo, TransA, Diag, CInt(X.length()), CInt(A.columns()), A, CInt(A.size(0)), X, X.stride(-1))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X)
				stbmv(order, Uplo, TransA, Diag, CInt(X.length()), CInt(A.columns()), A, CInt(A.size(0)), X, X.stride(-1))
			End If
		End Sub

		''' <summary>
		''' ?tbsv solves a system of linear equations whose coefficients are in a triangular band matrix.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		Public Overridable Sub tbsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal A As INDArray, ByVal X As INDArray) Implements Level2.tbsv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X)
			End If

			If X.length() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X)
				dtbsv(order, Uplo, TransA, Diag, CInt(X.length()), CInt(A.columns()), A, CInt(A.size(0)), X, X.stride(-1))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X)
				stbsv(order, Uplo, TransA, Diag, CInt(X.length()), CInt(A.columns()), A, CInt(A.size(0)), X, X.stride(-1))
			End If

		End Sub

		''' <summary>
		''' tpmv computes a matrix-vector product using a triangular packed matrix.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="Ap"> </param>
		''' <param name="X"> </param>
		Public Overridable Sub tpmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal Ap As INDArray, ByVal X As INDArray) Implements Level2.tpmv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, Ap, X)
			End If

			If Ap.length() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, X)
				dtpmv(order, Uplo, TransA, Diag, CInt(Ap.length()), Ap, X, X.stride(-1))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, X)
				stpmv(order, Uplo, TransA, Diag, CInt(Ap.length()), Ap, X, X.stride(-1))
			End If

			OpExecutionerUtil.checkForAny(X)
		End Sub

		''' <summary>
		''' tpsv solves a system of linear equations whose coefficients are in a triangular packed matrix.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="Ap"> </param>
		''' <param name="X"> </param>
		Public Overridable Sub tpsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal Ap As INDArray, ByVal X As INDArray) Implements Level2.tpsv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, Ap, X)
			End If

			If X.length() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, X, Ap)
				dtpsv(order, Uplo, TransA, Diag, CInt(X.length()), Ap, X, X.stride(-1))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, Ap, X)
				stpsv(order, Uplo, TransA, Diag, CInt(X.length()), Ap, X, X.stride(-1))
			End If

			OpExecutionerUtil.checkForAny(X)
		End Sub

		''' <summary>
		''' trmv computes a matrix-vector product using a triangular matrix.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		Public Overridable Sub trmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal A As INDArray, ByVal X As INDArray) Implements Level2.trmv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X)
			End If

			If X.length() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If A.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X)
				dtrmv(order, Uplo, TransA, Diag, CInt(X.length()), A, CInt(A.size(0)), X, X.stride(-1))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X)
				strmv(order, Uplo, TransA, Diag, CInt(X.length()), A, CInt(A.size(0)), X, X.stride(-1))
			End If

			OpExecutionerUtil.checkForAny(X)
		End Sub

		''' <summary>
		''' trsv solves a system of linear equations whose coefficients are in a triangular matrix.
		''' </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		Public Overridable Sub trsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal A As INDArray, ByVal X As INDArray) Implements Level2.trsv
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, X)
			End If

			If A.length() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, X)
				dtrsv(order, Uplo, TransA, Diag, CInt(A.length()), A, CInt(A.size(0)), X, X.stride(-1))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, X)
				strsv(order, Uplo, TransA, Diag, CInt(A.length()), A, CInt(A.size(0)), X, X.stride(-1))
			End If

			OpExecutionerUtil.checkForAny(X)
		End Sub

	'    
	'     * ===========================================================================
	'     * Prototypes for level 2 BLAS
	'     * ===========================================================================
	'     

	'     
	'     * Routines with standard 4 prefixes (S, D, C, Z)
	'     
		Protected Friend MustOverride Sub sgemv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub sgbmv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal KL As Integer, ByVal KU As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub strmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub stbmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub stpmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub strsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub stbsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub stpsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub dgemv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub dgbmv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal KL As Integer, ByVal KU As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub dtrmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub dtbmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub dtpmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub dtrsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub dtbsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub dtpsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)

	'     
	'     * Routines with S and D prefixes only
	'     
		Protected Friend MustOverride Sub ssymv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub ssbmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub sspmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub sger(ByVal order As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)

		Protected Friend MustOverride Sub ssyr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal A As INDArray, ByVal lda As Integer)

		Protected Friend MustOverride Sub sspr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Ap As INDArray)

		Protected Friend MustOverride Sub ssyr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)

		Protected Friend MustOverride Sub sspr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray)

		Protected Friend MustOverride Sub dsymv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub dsbmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub dspmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub dger(ByVal order As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)

		Protected Friend MustOverride Sub dsyr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal A As INDArray, ByVal lda As Integer)

		Protected Friend MustOverride Sub dspr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Ap As INDArray)

		Protected Friend MustOverride Sub dsyr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)

		Protected Friend MustOverride Sub dspr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray)
	End Class



End Namespace